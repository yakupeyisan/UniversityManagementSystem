using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Application.Common.Behaviours;

public class ExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehaviour(
        ILogger<ExceptionHandlingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception in {RequestName}", typeof(TRequest).Name);

            // DomainException'ı Result.Failure() olarak dönebiliriz
            // Result<T> dönen handlers için
            var result = CreateFailureResult(ex.Message);
            return result as TResponse ?? throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }

    private static object CreateFailureResult(string message)
    {
        // Reflection - TResponse Result<T> mi kontrol et
        if (typeof(TResponse).IsGenericType)
        {
            var genericDef = typeof(TResponse).GetGenericTypeDefinition();
            if (genericDef == typeof(Result<>))
            {
                var resultType = typeof(TResponse);
                var method = resultType.GetMethod("Failure", new[] { typeof(string) });
                return method?.Invoke(null, new object[] { message })!;
            }
        }

        // Result mi?
        if (typeof(TResponse) == typeof(Result))
        {
            return Result.Failure(message);
        }

        throw new InvalidOperationException(
            $"Cannot create failure result for {typeof(TResponse).Name}");
    }
}