using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.TemplateQuestion;

public static class TemplateQuestionErrors
{
    public static Error InvalidQuestionIndex =>
        Error.Validation("Question.InvalidIndex", "Question index must be greater than zero.");

    public static Error InvalidCoordinates =>
        Error.Validation("Question.InvalidCoordinates", "Coordinates X and Y must be non-negative.");

    public static Error InvalidDimensions =>
        Error.Validation("Question.InvalidDimensions", "Width and Height must be greater than zero.");

    public static Error ModelAnswerRequired =>
        Error.Validation("Question.ModelAnswerRequired", "Model answer is required.");

    public static Error InvalidMark =>
        Error.Validation("Question.InvalidMark", "Mark must be greater than zero.");

    public static Error NotFound =>
        Error.NotFound("Question.NotFound", "Question does not exist.");

    public static Error DuplicateQuestionIndex =>
        Error.Conflict("Question.DuplicateIndex", "Question index already exists in this template.");
}
