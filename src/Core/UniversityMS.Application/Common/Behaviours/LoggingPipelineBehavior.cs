using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace UniversityMS.Application.Common.Behaviours;

public class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation(
            "İstek Başlangıcı: {RequestName} | CorrelationId: {CorrelationId}",
            requestName, correlationId);

        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();

            _logger.LogInformation(
                "İstek Başarılı: {RequestName} | Süre: {ElapsedMilliseconds}ms | CorrelationId: {CorrelationId}",
                requestName, sw.ElapsedMilliseconds, correlationId);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();

            _logger.LogError(ex,
                "İstek Hatası: {RequestName} | Süre: {ElapsedMilliseconds}ms | CorrelationId: {CorrelationId} | Hata: {Message}",
                requestName, sw.ElapsedMilliseconds, correlationId, ex.Message);

            throw;
        }
    }
}