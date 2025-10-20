namespace UniversityMS.Infrastructure.Configuration;

public class SmsSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ApiUrl { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string SenderName { get; set; } = "UniversityManagementSystem";
}