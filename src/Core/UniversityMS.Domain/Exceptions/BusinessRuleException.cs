namespace UniversityMS.Domain.Exceptions;

public class BusinessRuleException : DomainException
{
    public string? RuleName { get; }

    public BusinessRuleException(string message, string? ruleName = null)
        : base(message)
    {
        RuleName = ruleName;
    }
}