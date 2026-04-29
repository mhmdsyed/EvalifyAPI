using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Application.Features.Questions.Dtos;
using Evalify.Application.Features.Questions.Mappers;
using Evalify.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Questions.Queries.GetQuestions;

public sealed class GetQuestionsQueryHandler(
    IAppDbContext db,
    ICurrentUser currentUser)
    : IRequestHandler<GetQuestionsQuery, Result<List<QuestionDto>>>
{
    public async Task<Result<List<QuestionDto>>> Handle(
        GetQuestionsQuery request,
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

        return questions.ToDtos();
    }
}
