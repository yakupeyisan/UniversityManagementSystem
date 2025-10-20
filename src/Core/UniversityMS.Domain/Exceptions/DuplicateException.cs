namespace UniversityMS.Domain.Exceptions;

public class DuplicateException : DomainException
{
    public string? DuplicateField { get; }

    public DuplicateException(string message, string? duplicateField = null)
        : base(message)
    {
        DuplicateField = duplicateField;
    }
}