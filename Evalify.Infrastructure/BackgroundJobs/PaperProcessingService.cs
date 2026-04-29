using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Entities.StudentAnswer;
using Evalify.Domain.Enums;
using Evalify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Evalify.Infrastructure.BackgroundJobs;

public sealed class PaperProcessingService(
    IServiceScopeFactory scopeFactory,
    ILogger<PaperProcessingService> logger)
    : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PaperProcessingService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueuedJobsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error in PaperProcessingService loop.");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }

        logger.LogInformation("PaperProcessingService stopped.");
    }

    private async Task ProcessQueuedJobsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var aiService = scope.ServiceProvider.GetRequiredService<IAiService>();

        var queuedJobs = await db.ProcessingJobs
            .Where(j => j.Status == JobStatus.Queued)
            .Include(j => j.StudentPaper)
                .ThenInclude(p => p!.Template)
                    .ThenInclude(t => t!.Questions)
            .ToListAsync(ct);

        if (queuedJobs.Count == 0)
            return;

        logger.LogInformation("Found {Count} queued job(s) to process.", queuedJobs.Count);

        foreach (var job in queuedJobs)
        {
            await ProcessJobAsync(db, aiService, job.Id, ct);
        }
    }

    private async Task ProcessJobAsync(
        AppDbContext db,
        IAiService aiService,
        int jobId,
        CancellationToken ct)
    {
        var job = await db.ProcessingJobs
            .Include(j => j.StudentPaper)
                .ThenInclude(p => p!.Template)
                    .ThenInclude(t => t!.Questions)
            .FirstOrDefaultAsync(j => j.Id == jobId, ct);

        if (job is null || job.StudentPaper is null)
        {
            logger.LogWarning("Job {JobId} not found or has no paper.", jobId);
            return;
        }

        var paper = job.StudentPaper;
        var questions = paper.Template?.Questions?.OrderBy(q => q.QuestionIndex).ToList();

        if (questions is null || questions.Count == 0)
        {
            logger.LogWarning(
                "Job {JobId}: No questions found for template {TemplateId}.",
                jobId,
                paper.TemplateId);
            await FailJobAsync(db, job, paper, ct);
            return;
        }

        job.MarkAsRunning();
        paper.MarkAsProcessing();
        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "Processing job {JobId} for paper {PaperId} (StudentCode: {Code}).",
            jobId,
            paper.Id,
            paper.StudentCode);

        try
        {
            if (!File.Exists(paper.ImagePath))
            {
                logger.LogError("Job {JobId}: Image file not found at {Path}.", jobId, paper.ImagePath);
                await FailJobAsync(db, job, paper, ct);
                return;
            }

            using var fullImage = await Image.LoadAsync(paper.ImagePath, ct);

            var answers = new List<StudentAnswer>();

            foreach (var question in questions)
            {
                try
                {
                    var cropRect = NormalizeCropRectangle(
                        question.X,
                        question.Y,
                        question.Width,
                        question.Height,
                        fullImage.Width,
                        fullImage.Height);

                    if (cropRect is null)
                    {
                        logger.LogWarning(
                            "Job {JobId}: Invalid crop bounds for question {Index}.",
                            jobId,
                            question.QuestionIndex);
                        continue;
                    }

                    using var croppedImage = fullImage.Clone(ctx => ctx.Crop(cropRect.Value));

                    using var croppedStream = new MemoryStream();
                    await croppedImage.SaveAsJpegAsync(croppedStream, ct);
                    croppedStream.Position = 0;

                    var aiResult = await aiService.EvaluateAsync(
                        croppedStream,
                        question.ModelAnswer,
                        question.Mark,
                        ct);

                    if (!aiResult.Success)
                    {
                        logger.LogWarning(
                            "Job {JobId}: AI returned failure for question {Index}.",
                            jobId,
                            question.QuestionIndex);

                        var failedAnswer = StudentAnswer.Create(
                            paper.Id,
                            question.Id,
                            0,
                            null);

                        if (failedAnswer.IsSuccess)
                            answers.Add(failedAnswer.Value);

                        continue;
                    }

                    var answerResult = StudentAnswer.Create(
                        paper.Id,
                        question.Id,
                        aiResult.Grade,
                        aiResult.ExtractedText);

                    if (answerResult.IsError)
                    {
                        logger.LogWarning(
                            "Job {JobId}: Failed to create answer for question {Index}: {Error}",
                            jobId,
                            question.QuestionIndex,
                            answerResult.TopError.Description);
                        continue;
                    }

                    answers.Add(answerResult.Value);

                    logger.LogInformation(
                        "Job {JobId}: Question {Index} graded - {Grade}/{MaxMark}",
                        jobId,
                        question.QuestionIndex,
                        aiResult.Grade,
                        question.Mark);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Job {JobId}: Error processing question {Index}.",
                        jobId,
                        question.QuestionIndex);
                }
            }

            db.StudentAnswers.AddRange(answers);
            await db.SaveChangesAsync(ct);

            var totalGrade = answers.Sum(a => a.Grade);

            paper.MarkAsDone(totalGrade);
            job.MarkAsDone();
            await db.SaveChangesAsync(ct);

            logger.LogInformation(
                "Job {JobId} completed. TotalGrade: {Total} for StudentCode: {Code}",
                jobId,
                totalGrade,
                paper.StudentCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job {JobId}: Critical failure during processing.", jobId);
            await FailJobAsync(db, job, paper, ct);
        }
    }

    private static async Task FailJobAsync(
        AppDbContext db,
        Domain.Entities.ProcessingJob.ProcessingJob job,
        Domain.Entities.StudentPaper.StudentPaper paper,
        CancellationToken ct)
    {
        job.MarkAsFailed();
        paper.MarkAsFailed();
        await db.SaveChangesAsync(ct);
    }

    private static Rectangle? NormalizeCropRectangle(
        int x,
        int y,
        int width,
        int height,
        int imageWidth,
        int imageHeight)
    {
        if (width <= 0 || height <= 0 || imageWidth <= 0 || imageHeight <= 0)
            return null;

        var safeX = Math.Clamp(x, 0, imageWidth - 1);
        var safeY = Math.Clamp(y, 0, imageHeight - 1);
        var maxWidth = imageWidth - safeX;
        var maxHeight = imageHeight - safeY;
        var safeWidth = Math.Min(width, maxWidth);
        var safeHeight = Math.Min(height, maxHeight);

        if (safeWidth <= 0 || safeHeight <= 0)
            return null;

        return new Rectangle(safeX, safeY, safeWidth, safeHeight);
    }
}
