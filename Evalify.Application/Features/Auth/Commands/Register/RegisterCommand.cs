using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<Result<RegisterResponse>>;

public sealed record RegisterResponse(
    string UserId,
    string FullName,
    string Email);
