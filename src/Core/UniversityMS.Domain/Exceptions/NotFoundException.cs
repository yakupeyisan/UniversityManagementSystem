namespace UniversityMS.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public string ResourceName { get; }
    public string ResourceId { get; }

    public NotFoundException(string resourceName, string resourceId)
        : base($"{resourceName} (ID: {resourceId}) bulunamadı.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    public NotFoundException(string resourceName, int resourceId)
        : this(resourceName, resourceId.ToString()) { }
}