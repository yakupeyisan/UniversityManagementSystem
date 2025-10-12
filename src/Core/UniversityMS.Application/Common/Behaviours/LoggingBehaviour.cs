using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;
        var username = _currentUserService.Username;

        _logger.LogInformation("UniversityMS Request: {Name} {@UserId} {@Username} {@Request}",
            requestName, userId, username, request);

        var response = await next();

        _logger.LogInformation("UniversityMS Response: {Name} {@Response}", requestName, response);

        return response;
    }
}