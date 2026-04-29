using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Application.Features.Results.Dtos;
using Evalify.Application.Features.Results.Mappers;
using Evalify.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Results.Queries.GetResults;

public sealed class GetResultsQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<GetResultsQuery, Result<ResultDto>>
{
    public async Task<Result<ResultDto>> Handle(
        GetResultsQuery request,
        CancellationToken ct)
    {
        var paper = await db.StudentPapers
            .Include(p => p.Template)
            .Include(p => p.Answers)
                .ThenInclude(a => a.Question)
            .FirstOrDefaultAsync(p => p.Id == request.StudentPaperId, ct);

        if (paper is null)
            return ApplicationErrors.PaperNotFound;

        if (paper.Template!.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        var answerDtos = paper.Answers
            .OrderBy(a => a.Question!.QuestionIndex)
            .Select(a => a.ToResultDto(
                a.Question!.QuestionIndex,
                a.Question!.ModelAnswer,
                a.Question!.Mark))
            .ToList();

        return new ResultDto(
            paper.Id,
            paper.StudentCode,
            paper.TotalGrade,
            paper.ImageUrl,
            paper.Status,
            answerDtos);
    }
}
