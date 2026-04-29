using Evalify.Application.Features.Results.Dtos;
using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Results.Queries.GetResults;

public sealed record GetResultsQuery(int StudentPaperId) : IRequest<Result<ResultDto>>;
