using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.StudentPaper;

public static class StudentPaperErrors
{
    public static Error StudentCodeRequired =>
        Error.Validation("Paper.StudentCodeRequired", "Student code is required.");

    public static Error ImageRequired =>
        Error.Validation("Paper.ImageRequired", "Paper image is required.");

    public static Error NotFound =>
        Error.NotFound("Paper.NotFound", "Student paper does not exist.");

    public static Error InvalidStatusTransition =>
        Error.Conflict("Paper.InvalidStatusTransition", "Invalid paper status transition.");

    public static Error InvalidTotalGrade =>
        Error.Validation("Paper.InvalidTotalGrade", "Total grade cannot be negative.");

    public static Error AlreadyProcessed =>
        Error.Conflict("Paper.AlreadyProcessed", "This paper has already been processed.");
}
