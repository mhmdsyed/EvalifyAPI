using Evalify.Application.Features.Templates.Dtos;
using Evalify.Domain.Entities.Template;

namespace Evalify.Application.Features.Templates.Mappers;

public static class TemplateMapper
{
    public static TemplateDto ToDto(this Template entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TemplateDto(
            entity.Id,
            entity.Name,
            entity.ImageUrl,
            entity.Width,
            entity.Height,
            entity.CreatedAt);
    }

    public static List<TemplateDto> ToDtos(this IEnumerable<Template> entities)
        => [.. entities.Select(e => e.ToDto())];
}
