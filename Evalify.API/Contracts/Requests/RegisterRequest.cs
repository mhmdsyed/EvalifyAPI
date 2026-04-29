namespace Evalify.API.Contracts.Requests;

public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Password);
