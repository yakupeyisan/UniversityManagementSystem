namespace UniversityMS.Domain.Interfaces;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; }

    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}