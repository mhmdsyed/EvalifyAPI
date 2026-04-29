using Evalify.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evalify.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse>(
    ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        try
        {
            return await next(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unhandled Exception for Request {RequestName} {@Request}",
                typeof(TRequest).Name, request);

            throw;
        }
    }
}
