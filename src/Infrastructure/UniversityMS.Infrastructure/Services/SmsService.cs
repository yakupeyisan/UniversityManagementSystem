using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Infrastructure.Configuration;

namespace UniversityMS.Infrastructure.Services;

public class SmsService : ISmsService
{
    private readonly SmsSettings _settings;
    private readonly ILogger<SmsService> _logger;
    private readonly HttpClient _httpClient;

    public SmsService(IOptions<SmsSettings> settings, ILogger<SmsService> logger, HttpClient httpClient)
    {
        _settings = settings.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendSmsAsync(string phoneNumber, string message)
    {
        var list= new[] { phoneNumber };
        await SendBulkSmsAsync(new[] { phoneNumber }, message);
    }

    public async Task SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message)
    {
        try
        {
            foreach (var phoneNumber in phoneNumbers)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _settings.ApiUrl)
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "api_key", _settings.ApiKey },
                        { "api_secret", _settings.ApiSecret },
                        { "to", phoneNumber },
                        { "from", _settings.SenderName },
                        { "text", message }
                    })
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("SMS sent successfully to {PhoneNumber}", phoneNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS");
            throw;
        }
    }
}