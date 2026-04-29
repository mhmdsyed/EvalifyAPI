using Evalify.Application.Features.Papers.Dtos;
using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Papers.Queries.GetPapers;

public sealed record GetPapersQuery(int TemplateId) : IRequest<Result<List<PaperDto>>>;
