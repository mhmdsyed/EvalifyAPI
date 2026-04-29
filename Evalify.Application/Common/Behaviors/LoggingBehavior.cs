using Evalify.Application.Common.Interfaces;
using Evalify.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evalify.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        string userId;

        try
        {
            userId = currentUser.Id;
        }
        catch
        {
            userId = "anonymous";
        }

        logger.LogInformation(
            "Handling {RequestName} for UserId: {UserId}",
            requestName, userId);

        var response = await next(ct);

        if (response.IsSuccess)
        {
            logger.LogInformation(
                "Handled {RequestName} successfully for UserId: {UserId}",
                requestName, userId);
        }
        else
        {
            logger.LogWarning(
                "Handled {RequestName} with errors for UserId: {UserId} - {Errors}",
                requestName, userId,
                string.Join(", ", response.Errors?.Select(e => e.Description) ?? []));
        }

        return response;
    }
}
