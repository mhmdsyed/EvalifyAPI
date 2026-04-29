using Evalify.Application.Features.Templates.Dtos;
using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Templates.Commands.CreateTemplate;

public sealed record CreateTemplateCommand(
    string Name,
    Stream ImageStream,
    string FileName) : IRequest<Result<TemplateDto>>;
