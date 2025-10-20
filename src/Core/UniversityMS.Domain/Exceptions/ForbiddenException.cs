namespace UniversityMS.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public string? UserId { get; }
    public string? RequiredRole { get; }

    public ForbiddenException(string message, string? userId = null,
        string? requiredRole = null)
        : base(message)
    {
        UserId = userId;
        RequiredRole = requiredRole;
    }
}