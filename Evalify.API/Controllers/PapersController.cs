using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.API.Controllers;

[Authorize]
[Route("api/papers")]
public sealed class PapersController : ApiController
{
}
