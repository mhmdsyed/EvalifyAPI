using Evalify.Domain.Entities.User;

namespace Evalify.Application.Common.Interfaces;

public interface IJwtService
{
    string? GenerateToken(User user);
}
