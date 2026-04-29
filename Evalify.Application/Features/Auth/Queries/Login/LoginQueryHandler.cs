using Evalify.Application.Common.Errors;
using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Evalify.Application.Features.Auth.Queries.Login;

public sealed class LoginQueryHandler(
    UserManager<User> userManager,
    IJwtService jwtService)
    : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return ApplicationErrors.InvalidCredentials;

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            return ApplicationErrors.InvalidCredentials;

        var token = jwtService.GenerateToken(user);
        if (token is null)
            return ApplicationErrors.TokenGenerationFailed;

        return new LoginResponse(token, user.Id, user.FullName, user.Email!);
    }
}
