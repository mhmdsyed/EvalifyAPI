using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.TemplateQuestion;

public sealed class TemplateQuestion
{
    private TemplateQuestion() { }

    public int Id { get; private set; }
    public int TemplateId { get; private set; }
    public int QuestionIndex { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string ModelAnswer { get; private set; } = string.Empty;
    public double Mark { get; private set; }

    public Template.Template? Template { get; private set; }
    public ICollection<StudentAnswer.StudentAnswer> StudentAnswers { get; private set; } = [];

    public static Result<TemplateQuestion> Create(
        int templateId,
        int questionIndex,
        int x,
        int y,
        int width,
        int height,
        string modelAnswer,
        double mark)
    {
        if (questionIndex <= 0)
            return TemplateQuestionErrors.InvalidQuestionIndex;

        if (x < 0 || y < 0)
            return TemplateQuestionErrors.InvalidCoordinates;

        if (width <= 0 || height <= 0)
            return TemplateQuestionErrors.InvalidDimensions;

        if (string.IsNullOrWhiteSpace(modelAnswer))
            return TemplateQuestionErrors.ModelAnswerRequired;

        if (mark <= 0)
            return TemplateQuestionErrors.InvalidMark;

        return new TemplateQuestion
        {
            TemplateId = templateId,
            QuestionIndex = questionIndex,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            ModelAnswer = modelAnswer.Trim(),
            Mark = mark
        };
    }

    public Result<Updated> UpdateGrade(double newMark)
    {
        if (newMark <= 0)
            return TemplateQuestionErrors.InvalidMark;

        Mark = newMark;
        return Result.Updated;
    }
}
