using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class MaterialAddedEvent : BaseDomainEvent
{
    public Guid MaterialId { get; }
    public string ISBN { get; }
    public string Title { get; }

    public MaterialAddedEvent(Guid materialId, string isbn, string title)
    {
        MaterialId = materialId;
        ISBN = isbn;
        Title = title;
    }
}