using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Common.Interfaces;

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message);
}