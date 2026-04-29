using System.Security.Claims;
using Evalify.Application.Common.Interfaces;

namespace Evalify.API.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string Id =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User is not authenticated.");
}
