using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Entities.ProcessingJob;
using Evalify.Domain.Entities.StudentPaper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evalify.Application.Features.Papers.Commands.UploadPaper;

public sealed class UploadPaperCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser,
    IFileStorage fileStorage)
    : IRequestHandler<UploadPaperCommand, Result<UploadPaperResponse>>
{
    public async Task<Result<UploadPaperResponse>> Handle(
        UploadPaperCommand request,
        CancellationToken ct)
    {
        var template = await db.Templates
            .FirstOrDefaultAsync(t => t.Id == request.TemplateId, ct);

        if (template is null)
            return ApplicationErrors.TemplateNotFound;

        if (template.UserId != currentUser.Id)
            return ApplicationErrors.TemplateNotOwnedByUser;

        var studentCode = Path.GetFileNameWithoutExtension(request.FileName);

        var exists = await db.StudentPapers
            .AnyAsync(p => p.TemplateId == request.TemplateId
                        && p.StudentCode == studentCode, ct);

        if (exists)
            return ApplicationErrors.PaperAlreadyExists;

        var (filePath, fileUrl) = await fileStorage.SaveAsync(
            request.ImageStream,
            request.FileName,
            "papers",
            ct);

        var paperResult = StudentPaper.Create(
            request.TemplateId,
            studentCode,
            filePath,
            fileUrl);

        if (paperResult.IsError)
            return paperResult.Errors;

        db.StudentPapers.Add(paperResult.Value);
        await db.SaveChangesAsync(ct);

        var jobResult = ProcessingJob.Create(paperResult.Value.Id);
        if (jobResult.IsError)
            return jobResult.Errors;

        db.ProcessingJobs.Add(jobResult.Value);
        await db.SaveChangesAsync(ct);

        return new UploadPaperResponse(
            paperResult.Value.Id,
            studentCode,
            paperResult.Value.Status.ToString());
    }
}
