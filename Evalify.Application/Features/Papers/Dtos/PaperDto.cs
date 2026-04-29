using Evalify.Domain.Enums;

namespace Evalify.Application.Features.Papers.Dtos;

public sealed record PaperDto(
    int StudentPaperId,
    string StudentCode,
    PaperStatus Status,
    double? TotalGrade,
    DateTime CreatedAt);
