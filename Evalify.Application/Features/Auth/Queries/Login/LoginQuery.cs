using Evalify.Domain.Common.Results;
using MediatR;

namespace Evalify.Application.Features.Auth.Queries.Login;

public sealed record LoginQuery(
    string Email,
    string Password) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(
    string Token,
    string UserId,
    string FullName,
    string Email);
