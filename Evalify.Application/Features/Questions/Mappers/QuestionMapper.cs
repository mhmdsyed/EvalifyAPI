using Evalify.Application.Features.Questions.Dtos;
using Evalify.Domain.Entities.TemplateQuestion;

namespace Evalify.Application.Features.Questions.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToDto(this TemplateQuestion entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new QuestionDto(
            entity.Id,
            entity.QuestionIndex,
            entity.X,
            entity.Y,
            entity.Width,
            entity.Height,
            entity.ModelAnswer,
            entity.Mark);
    }

    public static List<QuestionDto> ToDtos(this IEnumerable<TemplateQuestion> entities)
        => [.. entities.Select(e => e.ToDto())];
}
