using Evalify.Domain.Common.Results;
using Evalify.Domain.Common.Results.Abstractions;
using FluentValidation;
using MediatR;

namespace Evalify.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        if (errors.Count is 0)
            return await next(cancellationToken);

        return CreateValidationResult<TResponse>(errors);
    }

    private static TResponse CreateValidationResult<TResult>(List<Error> errors)
        where TResult : IResult
    {
        // Result<T> has implicit operator from List<Error>
        // We use reflection to create the correct Result<T>
        var resultType = typeof(TResult);

        if (resultType.IsGenericType)
        {
            var genericArg = resultType.GetGenericArguments()[0];
            var resultGenericType = typeof(Result<>).MakeGenericType(genericArg);
            var instance = (TResponse)Activator.CreateInstance(
                resultGenericType,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                [errors],
                null)!;
            return instance;
        }

        return default!;
    }
}
