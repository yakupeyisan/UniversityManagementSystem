using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Menü deaktif edildiğinde tetiklenen event
/// </summary>
public class MenuDeactivatedEvent : BaseDomainEvent
{
    public Guid MenuId { get; }
    public string MenuName { get; }

    public MenuDeactivatedEvent(Guid menuId, string menuName)
    {
        MenuId = menuId;
        MenuName = menuName;
    }
}