namespace UniversityMS.Domain.Exceptions;

public class ValidationException : DomainException
{
    public Dictionary<string, List<string>> Errors { get; }

    public ValidationException(string message,
        Dictionary<string, List<string>>? errors = null)
        : base(message)
    {
        Errors = errors ?? new();
    }

    public ValidationException(Dictionary<string, List<string>> errors)
        : base("Bir veya daha fazla doğrulama hatası oluştu.")
    {
        Errors = errors;
    }
}