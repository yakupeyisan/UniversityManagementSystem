using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = UniversityMS.Domain.Exceptions.ValidationException;

namespace UniversityMS.Application.Common.Behaviours;


public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationPipelineBehavior<TRequest, TResponse>> _logger;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToList());

        if (failures.Any())
        {
            _logger.LogWarning("Doğrulama hatası - {RequestType}: {Errors}",
                typeof(TRequest).Name, string.Join(", ", failures.Keys));

            throw new ValidationException("Doğrulama başarısız.", failures);
        }

        return await next();
    }
}