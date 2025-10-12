using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class PerformanceReviewAddedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid ReviewId { get; }
    public string ReviewPeriod { get; }

    public PerformanceReviewAddedEvent(Guid employeeId, Guid reviewId, string reviewPeriod)
    {
        EmployeeId = employeeId;
        ReviewId = reviewId;
        ReviewPeriod = reviewPeriod;
    }
}