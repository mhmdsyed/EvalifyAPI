using Evalify.Application.Common.Interfaces;
using Evalify.Application.Features.Templates.Dtos;
using Evalify.Application.Features.Templates.Mappers;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Entities.Template;
using MediatR;
using SixLabors.ImageSharp;

namespace Evalify.Application.Features.Templates.Commands.CreateTemplate;

public sealed class CreateTemplateCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser,
    IFileStorage fileStorage)
    : IRequestHandler<CreateTemplateCommand, Result<TemplateDto>>
{
    public async Task<Result<TemplateDto>> Handle(
        CreateTemplateCommand request,
        CancellationToken ct)
    {
        ImageInfo? imageInfo;
        try
        {
            imageInfo = await Image.IdentifyAsync(request.ImageStream, ct);
        }
        catch (SixLabors.ImageSharp.UnknownImageFormatException)
        {
            return Error.Validation("Template.ImageInvalid", "Uploaded file is not a valid image.");
        }

        if (imageInfo is null)
            return Error.Validation("Template.ImageInvalid", "Invalid image file.");

        int width = imageInfo.Width;
        int height = imageInfo.Height;

        request.ImageStream.Position = 0;

        var (filePath, fileUrl) = await fileStorage.SaveAsync(
            request.ImageStream,
            request.FileName,
            "templates",
            ct);

        var templateResult = Template.Create(
            currentUser.Id,
            request.Name,
            filePath,
            fileUrl,
            width,
            height);

        if (templateResult.IsError)
            return templateResult.Errors;

        db.Templates.Add(templateResult.Value);
        await db.SaveChangesAsync(ct);

        return templateResult.Value.ToDto();
    }
}
