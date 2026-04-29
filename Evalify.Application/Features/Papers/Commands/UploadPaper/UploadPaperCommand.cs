using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Papers.Commands.UploadPaper;

public sealed record UploadPaperCommand(
    int TemplateId,
    Stream ImageStream,
    string FileName) : IRequest<Result<UploadPaperResponse>>;

public sealed record UploadPaperResponse(
    int StudentPaperId,
    string StudentCode,
    string Status);
