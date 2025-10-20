using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Menü oluşturulduğunda tetiklenen event
/// </summary>
public class MenuCreatedEvent : BaseDomainEvent
{
    public Guid MenuId { get; }
    public string Name { get; }
    public MenuType Type { get; }

    public MenuCreatedEvent(Guid menuId, string name, MenuType type)
    {
        MenuId = menuId;
        Name = name;
        Type = type;
    }
}