using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class SupplierRatingUpdatedEvent : BaseDomainEvent
{
    public Guid SupplierId { get; }
    public decimal OldRating { get; }
    public decimal NewRating { get; }

    public SupplierRatingUpdatedEvent(Guid supplierId, decimal oldRating, decimal newRating)
    {
        SupplierId = supplierId;
        OldRating = oldRating;
        NewRating = newRating;
    }
}