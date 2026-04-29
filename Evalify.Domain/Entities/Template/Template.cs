using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.Template;

public sealed class Template
{
    private Template() { }

    public int Id { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ImagePath { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<TemplateQuestion.TemplateQuestion> Questions { get; private set; } = [];
    public ICollection<StudentPaper.StudentPaper> StudentPapers { get; private set; } = [];

    public static Result<Template> Create(
        string userId,
        string name,
        string imagePath,
        string imageUrl,
        int width,
        int height)
    {
        if (string.IsNullOrWhiteSpace(name))
            return TemplateErrors.NameRequired;

        if (name.Length > 100)
            return TemplateErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace(imagePath))
            return TemplateErrors.ImageRequired;

        if (width <= 0)
            return TemplateErrors.InvalidDimensions;

        if (height <= 0)
            return TemplateErrors.InvalidDimensions;

        return new Template
        {
            UserId = userId,
            Name = name.Trim(),
            ImagePath = imagePath,
            ImageUrl = imageUrl,
            Width = width,
            Height = height,
            CreatedAt = DateTime.UtcNow
        };
    }
}
