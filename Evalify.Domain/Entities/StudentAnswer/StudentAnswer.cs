using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.StudentAnswer;

public sealed class StudentAnswer
{
    private StudentAnswer() { }

    public int Id { get; private set; }
    public int StudentPaperId { get; private set; }
    public int QuestionId { get; private set; }
    public double Grade { get; private set; }
    public string? ExtractedText { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public StudentPaper.StudentPaper? StudentPaper { get; private set; }
    public TemplateQuestion.TemplateQuestion? Question { get; private set; }

    public static Result<StudentAnswer> Create(
        int studentPaperId,
        int questionId,
        double grade,
        string? extractedText)
    {
        if (grade < 0)
            return StudentAnswerErrors.InvalidGrade;

        return new StudentAnswer
        {
            StudentPaperId = studentPaperId,
            QuestionId = questionId,
            Grade = grade,
            ExtractedText = extractedText?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result<Updated> AdjustGrade(double newGrade)
    {
        if (newGrade < 0)
            return StudentAnswerErrors.InvalidGrade;

        Grade = newGrade;
        return Result.Updated;
    }
}
