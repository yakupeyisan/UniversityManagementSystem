using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

/// <summary>
/// Menü Maddesi
/// </summary>
public class MenuItem : AuditableEntity
{
    public Guid MenuId { get; private set; }
    public string Name { get; private set; } = null!;
    public MenuItemCategory Category { get; private set; }
    public Money Price { get; private set; } = null!;
    public int Calories { get; private set; }
    public bool IsVegetarian { get; private set; }
    public bool IsVegan { get; private set; }
    public int AvailableQuantity { get; private set; }
    public bool IsAvailable { get; private set; }

    public Menu Menu { get; private set; } = null!;

    private MenuItem() { }

    private MenuItem(Guid menuId, string name, MenuItemCategory category, Money price, int calories, int availableQty, bool isVegetarian, bool isVegan)
    {
        MenuId = menuId;
        Name = name;
        Category = category;
        Price = price;
        Calories = calories;
        AvailableQuantity = availableQty;
        IsVegetarian = isVegetarian;
        IsVegan = isVegan;
        IsAvailable = availableQty > 0;
    }

    public static MenuItem Create(Guid menuId, string name, MenuItemCategory category, Money price, int calories, int availableQty, bool isVegetarian = false, bool isVegan = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Madde adı boş olamaz.");
        if (price.Amount <= 0)
            throw new DomainException("Fiyat pozitif olmalıdır.");

        return new MenuItem(menuId, name, category, price, calories, availableQty, isVegetarian, isVegan);
    }

    public void DecreaseQuantity(int qty)
    {
        if (qty <= 0 || AvailableQuantity < qty)
            throw new DomainException("Yeterli stok yok.");
        AvailableQuantity -= qty;
        IsAvailable = AvailableQuantity > 0;
    }

    public void IncreaseQuantity(int qty)
    {
        if (qty <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");
        AvailableQuantity += qty;
        IsAvailable = true;
    }
}