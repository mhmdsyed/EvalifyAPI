using Evalify.Domain.Common.Results;

namespace Evalify.Domain.Entities.User;

public static class UserErrors
{
    public static Error FullNameRequired =>
        Error.Validation("User.FullNameRequired", "Full name is required.");

    public static Error FullNameTooLong =>
        Error.Validation("User.FullNameTooLong", "Full name cannot exceed 100 characters.");

    public static Error EmailRequired =>
        Error.Validation("User.EmailRequired", "Email is required.");

    public static Error NotFound =>
        Error.NotFound("User.NotFound", "User does not exist.");

    public static Error EmailAlreadyExists =>
        Error.Conflict("User.EmailAlreadyExists", "Email is already registered.");

    public static Error InvalidCredentials =>
        Error.Unauthorized("User.InvalidCredentials", "Invalid email or password.");
}
