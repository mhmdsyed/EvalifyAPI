using Evalify.Application.Common.Errors;
using Evalify.Domain.Common.Results;
using Evalify.Domain.Entities.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Evalify.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    UserManager<User> userManager)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return ApplicationErrors.EmailAlreadyExists;

        var userResult = User.Create(request.FullName, request.Email);
        if (userResult.IsError)
            return userResult.Errors;

        var user = userResult.Value;

        var identityResult = await userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors
                .Select(e => Error.Validation(e.Code, e.Description))
                .ToList();
            return errors;
        }

        await userManager.AddToRoleAsync(user, "Teacher");

        return new RegisterResponse(user.Id, user.FullName, user.Email!);
    }
}
