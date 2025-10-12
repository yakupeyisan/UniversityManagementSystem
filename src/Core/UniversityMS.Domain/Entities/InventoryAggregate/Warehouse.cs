using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.InventoryEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.InventoryAggregate;


/// <summary>
/// Depo (Warehouse) - Aggregate Root (Her kampüs için ayrı)
/// </summary>
public class Warehouse : AuditableEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid CampusId { get; private set; }
    public Address Location { get; private set; } = null!;
    public WarehouseType Type { get; private set; }
    public WarehouseStatus Status { get; private set; }
    public Guid? ManagerId { get; private set; }
    public string? Description { get; private set; }

    // Collections
    private readonly List<StockItem> _stockItems = new();
    public IReadOnlyCollection<StockItem> StockItems => _stockItems.AsReadOnly();

    private Warehouse() { }

    private Warehouse(
        string code,
        string name,
        Guid campusId,
        Address location,
        WarehouseType type,
        Guid? managerId = null)
    {
        Code = code;
        Name = name;
        CampusId = campusId;
        Location = location;
        Type = type;
        Status = WarehouseStatus.Active;
        ManagerId = managerId;
    }

    public static Warehouse Create(
        string code,
        string name,
        Guid campusId,
        Address location,
        WarehouseType type,
        Guid? managerId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Depo kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Depo adı boş olamaz.");

        return new Warehouse(code, name, campusId, location, type, managerId);
    }

    public void AddStockItem(StockItem item)
    {
        if (item.WarehouseId != Id)
            throw new DomainException("Stok kalemi bu depoya ait değil.");

        _stockItems.Add(item);
        AddDomainEvent(new StockItemAddedEvent(Id, item.Id, item.ItemCode));
    }
}


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

/// <summary>
/// Stok Hareketi (StockMovement) Entity
/// </summary>
public class StockMovement : AuditableEntity
{
    public Guid StockItemId { get; private set; }
    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public Guid? RelatedEntityId { get; private set; } // PurchaseOrder, Transfer vs.
    public string? Notes { get; private set; }

    public StockItem StockItem { get; private set; } = null!;

    private StockMovement() { }

    private StockMovement(
        Guid stockItemId,
        StockMovementType type,
        decimal quantity,
        DateTime movementDate,
        string? referenceNumber = null,
        Guid? relatedEntityId = null)
    {
        StockItemId = stockItemId;
        Type = type;
        Quantity = quantity;
        MovementDate = movementDate;
        ReferenceNumber = referenceNumber;
        RelatedEntityId = relatedEntityId;
    }

    public static StockMovement Create(
        Guid stockItemId,
        StockMovementType type,
        decimal quantity,
        DateTime movementDate,
        string? referenceNumber = null,
        Guid? relatedEntityId = null)
    {
        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new StockMovement(stockItemId, type, quantity, movementDate, referenceNumber, relatedEntityId);
    }
}