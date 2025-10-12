﻿namespace UniversityMS.Domain.Exceptions;

public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}