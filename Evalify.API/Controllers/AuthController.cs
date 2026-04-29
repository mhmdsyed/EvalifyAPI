using Asp.Versioning;
using Evalify.API.Contracts.Requests;
using Evalify.Application.Features.Auth.Commands.Register;
using Evalify.Application.Features.Auth.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Evalify.API.Controllers;

[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public sealed class AuthController(ISender sender) : ApiController
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Register a new teacher account.")]
    [EndpointName("Register")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new RegisterCommand(request.FullName, request.Email, request.Password), ct);

        return result.Match(
            response => CreatedAtAction(nameof(Register), response),
            Problem);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Login and receive JWT token.")]
    [EndpointName("Login")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new LoginQuery(request.Email, request.Password), ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }
}
