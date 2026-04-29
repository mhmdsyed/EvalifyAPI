using Evalify.Domain.Common.Results;
using Evalify.Domain.Enums;

namespace Evalify.Domain.Entities.ProcessingJob;

public sealed class ProcessingJob
{
    private ProcessingJob() { }

    public int Id { get; private set; }
    public int StudentPaperId { get; private set; }
    public JobStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public StudentPaper.StudentPaper? StudentPaper { get; private set; }

    public static Result<ProcessingJob> Create(int studentPaperId)
    {
        if (studentPaperId <= 0)
            return ProcessingJobErrors.InvalidStudentPaperId;

        return new ProcessingJob
        {
            StudentPaperId = studentPaperId,
            Status = JobStatus.Queued,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result<Updated> MarkAsRunning()
    {
        if (Status != JobStatus.Queued)
            return ProcessingJobErrors.InvalidStatusTransition;

        Status = JobStatus.Running;
        return Result.Updated;
    }

    public Result<Updated> MarkAsDone()
    {
        if (Status != JobStatus.Running)
            return ProcessingJobErrors.InvalidStatusTransition;

        Status = JobStatus.Done;
        CompletedAt = DateTime.UtcNow;
        return Result.Updated;
    }

    public Result<Updated> MarkAsFailed()
    {
        Status = JobStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        return Result.Updated;
    }
}
