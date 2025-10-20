namespace UniversityMS.Domain.Exceptions;

public class InvalidOperationException : DomainException
{
    public InvalidOperationException(string message) : base(message) { }
}