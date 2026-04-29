using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.StudentAnswer;

public static class StudentAnswerErrors
{
    public static Error InvalidGrade =>
        Error.Validation("Answer.InvalidGrade", "Grade cannot be negative.");

    public static Error NotFound =>
        Error.NotFound("Answer.NotFound", "Student answer does not exist.");

    public static Error GradeExceedsMaxMark =>
        Error.Validation("Answer.GradeExceedsMaxMark", "Grade cannot exceed the maximum mark.");
}
