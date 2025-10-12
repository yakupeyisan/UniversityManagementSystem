using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.InventoryEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

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