using Evalify.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace Evalify.Domain.Entities.User;

public sealed class User : IdentityUser
{
    public string FullName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public ICollection<Template.Template> Templates { get; private set; } = [];

    public static Result<User> Create(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return UserErrors.FullNameRequired;

        if (fullName.Length > 100)
            return UserErrors.FullNameTooLong;

        if (string.IsNullOrWhiteSpace(email))
            return UserErrors.EmailRequired;

        var user = new User
        {
            FullName = fullName.Trim(),
            Email = email.Trim(),
            UserName = email.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        return user;
    }
}
