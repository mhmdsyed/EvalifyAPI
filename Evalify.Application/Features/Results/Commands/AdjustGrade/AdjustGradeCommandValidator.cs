using FluentValidation;

namespace Evalify.Application.Features.Results.Commands.AdjustGrade;

public sealed class AdjustGradeCommandValidator : AbstractValidator<AdjustGradeCommand>
{
    public AdjustGradeCommandValidator()
    {
        RuleFor(x => x.AnswerId)
            .GreaterThan(0).WithMessage("Answer ID is required.");

        RuleFor(x => x.NewGrade)
            .GreaterThanOrEqualTo(0).WithMessage("Grade cannot be negative.");
    }
}
