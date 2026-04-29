using Evalify.Domain.Enums;

namespace Evalify.Application.Features.Results.Dtos;

public sealed record ResultDto(
    int StudentPaperId,
    string StudentCode,
    double? TotalGrade,
    string ImageUrl,
    PaperStatus Status,
    List<AnswerResultDto> Answers);

public sealed record AnswerResultDto(
    int AnswerId,
    int QuestionIndex,
    string ModelAnswer,
    string? ExtractedText,
    double Grade,
    double MaxMark);
