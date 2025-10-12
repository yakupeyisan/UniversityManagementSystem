using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.InventoryEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.InventoryAggregate;

/// <summary>
/// Stok Kalemi (StockItem) Entity
/// </summary>
public class StockItem : AuditableEntity
{
    public Guid WarehouseId { get; private set; }
    public string ItemCode { get; private set; } = null!;
    public string ItemName { get; private set; } = null!;
    public string? Description { get; private set; }
    public StockCategory Category { get; private set; }
    public string Unit { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public decimal MinimumStock { get; private set; }
    public decimal MaximumStock { get; private set; }
    public Money UnitCost { get; private set; } = null!;
    public string? Location { get; private set; } // Raf/Konum
    public string? Barcode { get; private set; }
    public DateTime? LastStockDate { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;

    // Collections
    private readonly List<StockMovement> _movements = new();
    public IReadOnlyCollection<StockMovement> Movements => _movements.AsReadOnly();

    private StockItem() { }

    private StockItem(
        Guid warehouseId,
        string itemCode,
        string itemName,
        StockCategory category,
        string unit,
        decimal minimumStock,
        decimal maximumStock,
        Money unitCost,
        string? description = null)
    {
        WarehouseId = warehouseId;
        ItemCode = itemCode;
        ItemName = itemName;
        Category = category;
        Unit = unit;
        Quantity = 0;
        MinimumStock = minimumStock;
        MaximumStock = maximumStock;
        UnitCost = unitCost;
        Description = description;
    }

    public static StockItem Create(
        Guid warehouseId,
        string itemCode,
        string itemName,
        StockCategory category,
        string unit,
        decimal minimumStock,
        decimal maximumStock,
        Money unitCost,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainException("Stok kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Stok adı boş olamaz.");

        if (minimumStock < 0)
            throw new DomainException("Minimum stok negatif olamaz.");

        if (maximumStock < minimumStock)
            throw new DomainException("Maksimum stok minimum stoktan küçük olamaz.");

        return new StockItem(warehouseId, itemCode, itemName, category, unit, minimumStock, maximumStock, unitCost, description);
    }

    public void AddMovement(StockMovement movement)
    {
        _movements.Add(movement);

        // Stok miktarını güncelle
        if (movement.Type == StockMovementType.In)
            Quantity += movement.Quantity;
        else if (movement.Type == StockMovementType.Out)
            Quantity -= movement.Quantity;

        LastStockDate = DateTime.UtcNow;

        AddDomainEvent(new StockMovementRecordedEvent(Id, movement.Type, movement.Quantity));
    }

    public bool IsBelowMinimum() => Quantity < MinimumStock;
    public bool IsAboveMaximum() => Quantity > MaximumStock;
    public bool IsOutOfStock() => Quantity <= 0;
}