using FluentValidation;

namespace Evalify.Application.Features.Papers.Commands.UploadPaper;

public sealed class UploadPaperCommandValidator : AbstractValidator<UploadPaperCommand>
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];

    public UploadPaperCommandValidator()
    {
        RuleFor(x => x.TemplateId)
            .GreaterThan(0).WithMessage("Template ID is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .Must(name => AllowedExtensions.Contains(Path.GetExtension(name).ToLower()))
            .WithMessage("Only JPG and PNG images are allowed.");
    }
}
