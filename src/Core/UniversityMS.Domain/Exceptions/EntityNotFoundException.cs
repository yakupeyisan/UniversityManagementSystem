namespace UniversityMS.Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} bulunamadı. Id: {id}")
    {
    }
}