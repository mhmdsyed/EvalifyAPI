using FluentValidation;

namespace Evalify.Application.Features.Questions.Commands.SaveQuestions;

public sealed class SaveQuestionsCommandValidator : AbstractValidator<SaveQuestionsCommand>
{
    public SaveQuestionsCommandValidator()
    {
        RuleFor(x => x.TemplateId)
            .GreaterThan(0).WithMessage("Template ID is required.");

        RuleFor(x => x.Questions)
            .NotEmpty().WithMessage("At least one question is required.");

        RuleForEach(x => x.Questions).ChildRules(q =>
        {
            q.RuleFor(x => x.QuestionIndex)
                .GreaterThan(0).WithMessage("Question index must be greater than zero.");

            q.RuleFor(x => x.X)
                .GreaterThanOrEqualTo(0).WithMessage("X coordinate must be non-negative.");

            q.RuleFor(x => x.Y)
                .GreaterThanOrEqualTo(0).WithMessage("Y coordinate must be non-negative.");

            q.RuleFor(x => x.Width)
                .GreaterThan(0).WithMessage("Width must be greater than zero.");

            q.RuleFor(x => x.Height)
                .GreaterThan(0).WithMessage("Height must be greater than zero.");

            q.RuleFor(x => x.ModelAnswer)
                .NotEmpty().WithMessage("Model answer is required.");

            q.RuleFor(x => x.Mark)
                .GreaterThan(0).WithMessage("Mark must be greater than zero.");
        });

        RuleFor(x => x.Questions)
            .Must(q => q.Select(x => x.QuestionIndex).Distinct().Count() == q.Count)
            .WithMessage("Question indexes must be unique.");
    }
}
