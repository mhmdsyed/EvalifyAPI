using Evalify.Application.Features.Papers.Dtos;
using Evalify.Domain.Entities.StudentPaper;

namespace Evalify.Application.Features.Papers.Mappers;

public static class PaperMapper
{
    public static PaperDto ToDto(this StudentPaper entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PaperDto(
            entity.Id,
            entity.StudentCode,
            entity.Status,
            entity.TotalGrade,
            entity.CreatedAt);
    }

    public static List<PaperDto> ToDtos(this IEnumerable<StudentPaper> entities)
        => [.. entities.Select(e => e.ToDto())];
}
