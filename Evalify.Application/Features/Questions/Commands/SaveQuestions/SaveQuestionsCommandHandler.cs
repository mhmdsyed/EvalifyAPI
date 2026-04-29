using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Entities.TemplateQuestion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Questions.Commands.SaveQuestions;

public sealed class SaveQuestionsCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<SaveQuestionsCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        SaveQuestionsCommand request,
        CancellationToken ct)
    {
        var template = await db.Templates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, ct);

        if (template is null)
            return ApplicationErrors.TemplateNotFound;

        if (template.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        var existing = await db.TemplateQuestions
            .Where(q => q.TemplateId == request.TemplateId)
            .ToListAsync(ct);

        db.TemplateQuestions.RemoveRange(existing);

        var errors = new List<Error>();
        var newQuestions = new List<TemplateQuestion>();

        foreach (var item in request.Questions)
        {
            var result = TemplateQuestion.Create(
                request.TemplateId,
                item.QuestionIndex,
                item.X,
                item.Y,
                item.Width,
                item.Height,
                item.ModelAnswer,
                item.Mark);

            if (result.IsError)
            {
                errors.AddRange(result.Errors);
                continue;
            }

            newQuestions.Add(result.Value);
        }

        if (errors.Count > 0)
            return errors;

        db.TemplateQuestions.AddRange(newQuestions);
        await db.SaveChangesAsync(ct);

        return Result.Updated;
    }
}
