namespace Evalify.Application.Features.Templates.Dtos;

public sealed record TemplateDto(
    int TemplateId,
    string Name,
    string ImageUrl,
    int Width,
    int Height,
    DateTime CreatedAt);
