using System.Text;
using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Templates.Queries.ExportResults;

public sealed class ExportResultsQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<ExportResultsQuery, Result<ExportResultsResponse>>
{
    public async Task<Result<ExportResultsResponse>> Handle(
        ExportResultsQuery request,
        CancellationToken ct)
    {
        var template = await db.Templates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, ct);

        if (template is null)
            return ApplicationErrors.TemplateNotFound;

        if (template.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        var questions = await db.TemplateQuestions
            .Where(q => q.TemplateId == request.TemplateId)
            .OrderBy(q => q.QuestionIndex)
            .ToListAsync(ct);

        var papers = await db.StudentPapers
            .Where(p => p.TemplateId == request.TemplateId && p.Status == PaperStatus.Done)
            .Include(p => p.Answers)
            .OrderBy(p => p.StudentCode)
            .ToListAsync(ct);

        var sb = new StringBuilder();

        sb.Append("StudentCode,");
        foreach (var q in questions)
        {
            sb.Append($"Q{q.QuestionIndex} Grade,");
            sb.Append($"Q{q.QuestionIndex} MaxMark,");
        }

        sb.Append("TotalGrade");
        sb.AppendLine();

        foreach (var paper in papers)
        {
            sb.Append($"{paper.StudentCode},");

            foreach (var question in questions)
            {
                var answer = paper.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
                var grade = answer?.Grade ?? 0;
                sb.Append($"{grade},");
                sb.Append($"{question.Mark},");
            }

            sb.Append($"{paper.TotalGrade ?? 0}");
            sb.AppendLine();
        }

        var fileContent = Encoding.UTF8.GetBytes(sb.ToString());
        var fileName = $"results_{template.Name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

        return new ExportResultsResponse(fileContent, fileName, "text/csv");
    }
}
