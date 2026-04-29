using Evalify.Application.Features.Templates.Dtos;
using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Templates.Queries.GetTemplates;

public sealed record GetTemplatesQuery : IRequest<Result<List<TemplateDto>>>;
