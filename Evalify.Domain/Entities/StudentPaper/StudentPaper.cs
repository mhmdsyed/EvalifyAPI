using Evalify.Domain.Common.Results;
using Evalify.Domain.Enums;

namespace Evalify.Domain.Entities.StudentPaper;

public sealed class StudentPaper
{
    private StudentPaper() { }

    public int Id { get; private set; }
    public int TemplateId { get; private set; }
    public string StudentCode { get; private set; } = string.Empty;
    public string ImagePath { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public PaperStatus Status { get; private set; }
    public double? TotalGrade { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Template.Template? Template { get; private set; }
    public ICollection<StudentAnswer.StudentAnswer> Answers { get; private set; } = [];
    public ProcessingJob.ProcessingJob? ProcessingJob { get; private set; }

    public static Result<StudentPaper> Create(
        int templateId,
        string studentCode,
        string imagePath,
        string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(studentCode))
            return StudentPaperErrors.StudentCodeRequired;

        if (string.IsNullOrWhiteSpace(imagePath))
            return StudentPaperErrors.ImageRequired;

        return new StudentPaper
        {
            TemplateId = templateId,
            StudentCode = studentCode.Trim(),
            ImagePath = imagePath,
            ImageUrl = imageUrl,
            Status = PaperStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result<Updated> MarkAsProcessing()
    {
        if (Status != PaperStatus.Pending)
            return StudentPaperErrors.InvalidStatusTransition;

        Status = PaperStatus.Processing;
        return Result.Updated;
    }

    public Result<Updated> MarkAsDone(double totalGrade)
    {
        if (Status != PaperStatus.Processing)
            return StudentPaperErrors.InvalidStatusTransition;

        if (totalGrade < 0)
            return StudentPaperErrors.InvalidTotalGrade;

        Status = PaperStatus.Done;
        TotalGrade = totalGrade;
        return Result.Updated;
    }

    public Result<Updated> MarkAsFailed()
    {
        Status = PaperStatus.Failed;
        return Result.Updated;
    }

    public Result<Updated> UpdateTotalGrade(double totalGrade)
    {
        if (totalGrade < 0)
            return StudentPaperErrors.InvalidTotalGrade;

        TotalGrade = totalGrade;
        return Result.Updated;
    }
}
