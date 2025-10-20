using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace UniversityMS.Application.Common.Behaviours;

public class CachingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableRequest
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachingPipelineBehavior<TRequest, TResponse>> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CachingPipelineBehavior(IDistributedCache cache,
        ILogger<CachingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = request.CacheKey;

        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Cache HIT: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<TResponse>(cachedData, JsonOptions)
                       ?? await next();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache okuma hatası: {CacheKey}", cacheKey);
        }

        _logger.LogInformation("Cache MISS: {CacheKey}", cacheKey);
        var response = await next();

        try
        {
            var cacheDuration = request.CacheDuration ?? TimeSpan.FromMinutes(5);
            var serializedData = JsonSerializer.Serialize(response, JsonOptions);

            await _cache.SetStringAsync(cacheKey, serializedData,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheDuration
                },
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache yazma hatası: {CacheKey}", cacheKey);
        }

        return response;
    }
}