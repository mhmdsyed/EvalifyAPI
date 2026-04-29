using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.Template;

public static class TemplateErrors
{
    public static Error NameRequired =>
        Error.Validation("Template.NameRequired", "Template name is required.");

    public static Error NameTooLong =>
        Error.Validation("Template.NameTooLong", "Template name cannot exceed 100 characters.");

    public static Error ImageRequired =>
        Error.Validation("Template.ImageRequired", "Template image is required.");

    public static Error InvalidDimensions =>
        Error.Validation("Template.InvalidDimensions", "Image dimensions must be greater than zero.");

    public static Error NotFound =>
        Error.NotFound("Template.NotFound", "Template does not exist.");

    public static Error NotOwnedByUser =>
        Error.Forbidden("Template.NotOwned", "You do not own this template.");
}
