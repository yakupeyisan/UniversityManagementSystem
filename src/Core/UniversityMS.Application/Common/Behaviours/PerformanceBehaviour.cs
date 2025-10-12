using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehaviour(
        ILogger<PerformanceBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500) // 500ms'den uzun süren requestleri logla
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var username = _currentUserService.Username;

            _logger.LogWarning(
                "UniversityMS Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Username} {@Request}",
                requestName, elapsedMilliseconds, userId, username, request);
        }

        return response;
    }
}