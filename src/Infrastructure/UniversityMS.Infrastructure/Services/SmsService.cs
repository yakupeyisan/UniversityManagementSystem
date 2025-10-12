using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        // SMS sending logic would be implemented here
        // Using Twilio, Netgsm, etc.
        _logger.LogInformation("SMS sent to {PhoneNumber}: {Message}", phoneNumber, message);
        await Task.CompletedTask;
    }

    public async Task SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message)
    {
        foreach (var phoneNumber in phoneNumbers)
        {
            await SendSmsAsync(phoneNumber, message);
        }
    }
}