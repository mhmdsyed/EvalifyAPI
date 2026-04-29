using FluentValidation;

namespace Evalify.Application.Features.Templates.Commands.CreateTemplate;

public sealed class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];

    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Template name is required.")
            .MaximumLength(100).WithMessage("Template name cannot exceed 100 characters.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("Image file is required.")
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLower()))
            .WithMessage("Only JPG and PNG images are allowed.");
    }
}
