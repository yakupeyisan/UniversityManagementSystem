namespace UniversityMS.Domain.Exceptions;

public class UnauthorizedAccessException : DomainException
{
    public string? UserId { get; }

    public UnauthorizedAccessException(string message, string? userId = null)
        : base(message)
    {
        UserId = userId;
    }
}