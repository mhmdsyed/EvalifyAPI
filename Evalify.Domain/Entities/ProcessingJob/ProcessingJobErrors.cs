using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.ProcessingJob;

public static class ProcessingJobErrors
{
    public static Error InvalidStudentPaperId =>
        Error.Validation("Job.InvalidPaperId", "Student paper ID must be greater than zero.");

    public static Error NotFound =>
        Error.NotFound("Job.NotFound", "Processing job does not exist.");

    public static Error InvalidStatusTransition =>
        Error.Conflict("Job.InvalidStatusTransition", "Invalid job status transition.");
}
