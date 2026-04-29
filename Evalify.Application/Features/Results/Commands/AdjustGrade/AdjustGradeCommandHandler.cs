using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Results.Commands.AdjustGrade;

public sealed class AdjustGradeCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<AdjustGradeCommand, Result<AdjustGradeResponse>>
{
    public async Task<Result<AdjustGradeResponse>> Handle(
        AdjustGradeCommand request,
        CancellationToken ct)
    {
        var answer = await db.StudentAnswers
            .Include(a => a.Question)
            .Include(a => a.StudentPaper)
                .ThenInclude(p => p!.Template)
            .FirstOrDefaultAsync(a => a.Id == request.AnswerId, ct);

        if (answer is null)
            return ApplicationErrors.AnswerNotFound;

        if (answer.StudentPaper!.Template!.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        if (request.NewGrade > answer.Question!.Mark)
            return ApplicationErrors.GradeExceedsMaxMark;

        var adjustResult = answer.AdjustGrade(request.NewGrade);
        if (adjustResult.IsError)
            return adjustResult.Errors;

        await db.SaveChangesAsync(ct);

        var totalGrade = await db.StudentAnswers
            .Where(a => a.StudentPaperId == answer.StudentPaperId)
            .SumAsync(a => a.Grade, ct);

        var updateResult = answer.StudentPaper.UpdateTotalGrade(totalGrade);
        if (updateResult.IsError)
            return updateResult.Errors;

        await db.SaveChangesAsync(ct);

        return new AdjustGradeResponse(totalGrade);
    }
}
