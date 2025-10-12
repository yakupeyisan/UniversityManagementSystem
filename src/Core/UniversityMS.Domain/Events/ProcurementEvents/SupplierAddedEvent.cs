using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class SupplierAddedEvent : BaseDomainEvent
{
    public Guid SupplierId { get; }
    public string Code { get; }
    public string Name { get; }

    public SupplierAddedEvent(Guid supplierId, string code, string name)
    {
        SupplierId = supplierId;
        Code = code;
        Name = name;
    }
}