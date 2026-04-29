using System.Diagnostics;
using Evalify.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evalify.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    private const int SlowRequestThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var timer = Stopwatch.StartNew();

        var response = await next(ct);

        timer.Stop();

        if (timer.ElapsedMilliseconds > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "Slow Request: {RequestName} took {ElapsedMs}ms - {@Request}",
                typeof(TRequest).Name,
                timer.ElapsedMilliseconds,
                request);
        }

        return response;
    }
}
