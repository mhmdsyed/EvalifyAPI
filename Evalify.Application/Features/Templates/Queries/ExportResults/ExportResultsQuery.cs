using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Templates.Queries.ExportResults;

public sealed record ExportResultsQuery(int TemplateId) : IRequest<Result<ExportResultsResponse>>;

public sealed record ExportResultsResponse(
    byte[] FileContent,
    string FileName,
    string ContentType);
