namespace UniversityMS.Domain.Filters;

public class FilterParsingException : Exception
{
    public FilterParsingException(string message) : base(message) { }
    public FilterParsingException(string message, Exception innerException)
        : base(message, innerException) { }
}