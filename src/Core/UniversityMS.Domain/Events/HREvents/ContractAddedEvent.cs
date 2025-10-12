using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class ContractAddedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid ContractId { get; }

    public ContractAddedEvent(Guid employeeId, Guid contractId)
    {
        EmployeeId = employeeId;
        ContractId = contractId;
    }
}