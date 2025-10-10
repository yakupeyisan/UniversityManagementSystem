using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Satın Alma Talebi (PurchaseRequest) - Aggregate Root
/// </summary>
public class PurchaseRequest : AuditableEntity, IAggregateRoot
{
    public string RequestNumber { get; private set; } = null!;
    public Guid RequestorId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime RequiredDate { get; private set; }
    public PurchaseRequestStatus Status { get; private set; }
    public PurchasePriority Priority { get; private set; }
    public string Justification { get; private set; } = null!;
    public Money EstimatedTotal { get; private set; } = null!;
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<PurchaseRequestItem> _items = new();
    public IReadOnlyCollection<PurchaseRequestItem> Items => _items.AsReadOnly();

    private PurchaseRequest() { }

    private PurchaseRequest(
        string requestNumber,
        Guid requestorId,
        Guid departmentId,
        DateTime requestDate,
        DateTime requiredDate,
        PurchasePriority priority,
        string justification)
    {
        RequestNumber = requestNumber;
        RequestorId = requestorId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        RequiredDate = requiredDate;
        Priority = priority;
        Justification = justification;
        Status = PurchaseRequestStatus.Draft;
        EstimatedTotal = Money.Zero();
    }

    public static PurchaseRequest Create(
        string requestNumber,
        Guid requestorId,
        Guid departmentId,
        DateTime requestDate,
        DateTime requiredDate,
        PurchasePriority priority,
        string justification)
    {
        if (string.IsNullOrWhiteSpace(requestNumber))
            throw new DomainException("Talep numarası boş olamaz.");

        if (requiredDate < requestDate)
            throw new DomainException("İhtiyaç tarihi talep tarihinden önce olamaz.");

        if (string.IsNullOrWhiteSpace(justification))
            throw new DomainException("Gerekçe belirtilmelidir.");

        return new PurchaseRequest(requestNumber, requestorId, departmentId, requestDate, requiredDate, priority, justification);
    }

    public void AddItem(PurchaseRequestItem item)
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new DomainException("Sadece taslak taleplere kalem eklenebilir.");

        _items.Add(item);
        RecalculateTotal();
    }

    public void Submit()
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new DomainException("Sadece taslak talepler gönderilebilir.");

        if (!_items.Any())
            throw new DomainException("En az bir kalem eklenmelidir.");

        Status = PurchaseRequestStatus.Submitted;
        AddDomainEvent(new PurchaseRequestSubmittedEvent(Id, RequestNumber));
    }

    public void Approve(Guid approverId)
    {
        if (Status != PurchaseRequestStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş talepler onaylanabilir.");

        Status = PurchaseRequestStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new PurchaseRequestApprovedEvent(Id, approverId));
    }

    public void Reject(string reason)
    {
        if (Status != PurchaseRequestStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş talepler reddedilebilir.");

        Status = PurchaseRequestStatus.Rejected;
        RejectionReason = reason;
    }

    private void RecalculateTotal()
    {
        EstimatedTotal = Money.Create(_items.Sum(i => i.EstimatedTotal.Amount), "TRY");
    }
}

/// <summary>
/// Satın Alma Talebi Kalemi Entity
/// </summary>
public class PurchaseRequestItem : AuditableEntity
{
    public Guid PurchaseRequestId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Specifications { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public Money EstimatedUnitPrice { get; private set; } = null!;
    public Money EstimatedTotal { get; private set; } = null!;

    public PurchaseRequest PurchaseRequest { get; private set; } = null!;

    private PurchaseRequestItem() { }

    private PurchaseRequestItem(
        Guid purchaseRequestId,
        string itemName,
        decimal quantity,
        string unit,
        Money estimatedUnitPrice,
        string? description = null,
        string? specifications = null)
    {
        PurchaseRequestId = purchaseRequestId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        EstimatedUnitPrice = estimatedUnitPrice;
        EstimatedTotal = Money.Create(quantity * estimatedUnitPrice.Amount, estimatedUnitPrice.Currency);
        Description = description;
        Specifications = specifications;
    }

    public static PurchaseRequestItem Create(
        Guid purchaseRequestId,
        string itemName,
        decimal quantity,
        string unit,
        Money estimatedUnitPrice,
        string? description = null,
        string? specifications = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Malzeme adı boş olamaz.");

        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Birim belirtilmelidir.");

        return new PurchaseRequestItem(purchaseRequestId, itemName, quantity, unit, estimatedUnitPrice, description, specifications);
    }
}

/// <summary>
/// Satın Alma Siparişi (PurchaseOrder) - Aggregate Root
/// </summary>
public class PurchaseOrder : AuditableEntity, IAggregateRoot
{
    public string OrderNumber { get; private set; } = null!;
    public Guid SupplierId { get; private set; }
    public Guid? PurchaseRequestId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public string? DeliveryAddress { get; private set; }
    public string? Terms { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<PurchaseOrderItem> _items = new();
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private PurchaseOrder() { }

    private PurchaseOrder(
        string orderNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        string? deliveryAddress = null,
        Guid? purchaseRequestId = null)
    {
        OrderNumber = orderNumber;
        SupplierId = supplierId;
        PurchaseRequestId = purchaseRequestId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Status = PurchaseOrderStatus.Draft;
        TotalAmount = Money.Zero();
        DeliveryAddress = deliveryAddress;
    }

    public static PurchaseOrder Create(
        string orderNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        string? deliveryAddress = null,
        Guid? purchaseRequestId = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("Sipariş numarası boş olamaz.");

        if (expectedDeliveryDate < orderDate)
            throw new DomainException("Teslimat tarihi sipariş tarihinden önce olamaz.");

        return new PurchaseOrder(orderNumber, supplierId, orderDate, expectedDeliveryDate, deliveryAddress, purchaseRequestId);
    }

    public void AddItem(PurchaseOrderItem item)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Sadece taslak siparişlere kalem eklenebilir.");

        _items.Add(item);
        RecalculateTotal();
    }

    public void Submit()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Sadece taslak siparişler gönderilebilir.");

        Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve(Guid approverId)
    {
        if (Status != PurchaseOrderStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş siparişler onaylanabilir.");

        Status = PurchaseOrderStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new PurchaseOrderApprovedEvent(Id, OrderNumber, SupplierId));
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Approved)
            throw new DomainException("Sadece onaylı siparişler teslim alınabilir.");

        Status = PurchaseOrderStatus.Received;
        AddDomainEvent(new PurchaseOrderReceivedEvent(Id, OrderNumber));
    }

    public void Complete()
    {
        if (Status != PurchaseOrderStatus.Received)
            throw new DomainException("Sipariş teslim alınmadan tamamlanamaz.");

        Status = PurchaseOrderStatus.Completed;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Money.Create(_items.Sum(i => i.TotalAmount.Amount), "TRY");
    }
}

/// <summary>
/// Satın Alma Siparişi Kalemi Entity
/// </summary>
public class PurchaseOrderItem : AuditableEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = null!;
    public decimal ReceivedQuantity { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    private PurchaseOrderItem() { }

    private PurchaseOrderItem(
        Guid purchaseOrderId,
        string itemName,
        decimal quantity,
        string unit,
        Money unitPrice,
        string? description = null)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        TotalAmount = Money.Create(quantity * unitPrice.Amount, unitPrice.Currency);
        ReceivedQuantity = 0;
        Description = description;
    }

    public static PurchaseOrderItem Create(
        Guid purchaseOrderId,
        string itemName,
        decimal quantity,
        string unit,
        Money unitPrice,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Malzeme adı boş olamaz.");

        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new PurchaseOrderItem(purchaseOrderId, itemName, quantity, unit, unitPrice, description);
    }

    public void RecordReceipt(decimal receivedQty)
    {
        if (receivedQty <= 0)
            throw new DomainException("Teslim alınan miktar pozitif olmalıdır.");

        if (ReceivedQuantity + receivedQty > Quantity)
            throw new DomainException("Teslim alınan miktar sipariş miktarını aşamaz.");

        ReceivedQuantity += receivedQty;
    }

    public bool IsFullyReceived() => ReceivedQuantity >= Quantity;
}

/// <summary>
/// Tedarikçi (Supplier) Entity
/// </summary>
public class Supplier : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? TaxNumber { get; private set; }
    public string? TaxOffice { get; private set; }
    public SupplierType Type { get; private set; }
    public SupplierStatus Status { get; private set; }
    public ContactInfo ContactInfo { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public string? BankAccount { get; private set; }
    public int PaymentTermDays { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public decimal Rating { get; private set; }
    public string? Notes { get; private set; }

    private Supplier() { }

    private Supplier(
        string code,
        string name,
        SupplierType type,
        ContactInfo contactInfo,
        Address address,
        int paymentTermDays = 30,
        string? taxNumber = null,
        string? taxOffice = null)
    {
        Code = code;
        Name = name;
        Type = type;
        Status = SupplierStatus.Active;
        ContactInfo = contactInfo;
        Address = address;
        PaymentTermDays = paymentTermDays;
        TaxNumber = taxNumber;
        TaxOffice = taxOffice;
        Rating = 0;
    }

    public static Supplier Create(
        string code,
        string name,
        SupplierType type,
        ContactInfo contactInfo,
        Address address,
        int paymentTermDays = 30,
        string? taxNumber = null,
        string? taxOffice = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Tedarikçi kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tedarikçi adı boş olamaz.");

        if (paymentTermDays < 0)
            throw new DomainException("Ödeme vadesi negatif olamaz.");

        return new Supplier(code, name, type, contactInfo, address, paymentTermDays, taxNumber, taxOffice);
    }

    public void UpdateRating(decimal rating)
    {
        if (rating < 0 || rating > 5)
            throw new DomainException("Değerlendirme 0-5 arasında olmalıdır.");

        Rating = rating;
    }

    public void Activate() => Status = SupplierStatus.Active;
    public void Deactivate() => Status = SupplierStatus.Inactive;
    public void Blacklist() => Status = SupplierStatus.Blacklisted;
}

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