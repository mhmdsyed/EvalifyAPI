using Evalify.Application.Features.Results.Dtos;
using Evalify.Domain.Entities.StudentAnswer;

namespace Evalify.Application.Features.Results.Mappers;

public static class ResultMapper
{
    public static AnswerResultDto ToResultDto(
        this StudentAnswer entity,
        int questionIndex,
        string modelAnswer,
        double maxMark)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AnswerResultDto(
            entity.Id,
            questionIndex,
            modelAnswer,
            entity.ExtractedText,
            entity.Grade,
            maxMark);
    }
}
