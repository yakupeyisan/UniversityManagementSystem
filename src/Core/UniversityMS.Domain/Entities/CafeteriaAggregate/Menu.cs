using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.CafeteriaEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

/// <summary>
/// Yemekhane Menüsü - Aggregate Root
/// </summary>
public class Menu : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public MenuType Type { get; private set; }
    public DateTime EffectiveDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public int DayOfWeek { get; private set; }

    private readonly List<MenuItem> _items = new();
    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();

    private Menu() { }

    private Menu(string name, MenuType type, DateTime effectiveDate, int dayOfWeek)
    {
        Name = name;
        Type = type;
        EffectiveDate = effectiveDate;
        DayOfWeek = dayOfWeek;
        IsActive = true;
    }

    public static Menu Create(string name, MenuType type, DateTime effectiveDate, int dayOfWeek)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Menü adı boş olamaz.");
        if (dayOfWeek < 0 || dayOfWeek > 6)
            throw new DomainException("Gün numarası 0-6 arasında olmalıdır.");

        var menu = new Menu(name, type, effectiveDate, dayOfWeek);
        menu.AddDomainEvent(new MenuCreatedEvent(menu.Id, name, type));
        return menu;
    }

    public void AddItem(MenuItem item)
    {
        if (_items.Any(x => x.Id == item.Id))
            throw new DomainException("Bu madde menüye zaten eklenmiştir.");
        _items.Add(item);
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item != null) _items.Remove(item);
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Menü zaten deaktiftir.");
        IsActive = false;
        EndDate = DateTime.UtcNow;
    }
}