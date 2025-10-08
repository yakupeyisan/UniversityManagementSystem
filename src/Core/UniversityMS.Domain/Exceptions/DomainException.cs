using System.Data;
using System.Net;
using System.Security;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} bulunamadı. Id: {id}")
    {
    }
}

public class BusinessRuleValidationException : DomainException
{
    public BusinessRuleValidationException(string message) : base(message)
    {
    }
}
