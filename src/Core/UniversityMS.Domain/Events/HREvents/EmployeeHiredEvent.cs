using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.HREvents;
public class EmployeeHiredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public EmployeeNumber EmployeeNumber { get; }
    public Guid PersonId { get; }
    public DateTime HireDate { get; }

    public EmployeeHiredEvent(Guid employeeId, EmployeeNumber employeeNumber, Guid personId, DateTime hireDate)
    {
        EmployeeId = employeeId;
        EmployeeNumber = employeeNumber;
        PersonId = personId;
        HireDate = hireDate;
    }
}

public class EmployeeTerminatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime TerminationDate { get; }
    public string Reason { get; }

    public EmployeeTerminatedEvent(Guid employeeId, DateTime terminationDate, string reason)
    {
        EmployeeId = employeeId;
        TerminationDate = terminationDate;
        Reason = reason;
    }
}

public class EmployeeResignedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime ResignationDate { get; }

    public EmployeeResignedEvent(Guid employeeId, DateTime resignationDate)
    {
        EmployeeId = employeeId;
        ResignationDate = resignationDate;
    }
}

public class EmployeeRetiredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime RetirementDate { get; }

    public EmployeeRetiredEvent(Guid employeeId, DateTime retirementDate)
    {
        EmployeeId = employeeId;
        RetirementDate = retirementDate;
    }
}

public class EmployeeSuspendedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public string Reason { get; }

    public EmployeeSuspendedEvent(Guid employeeId, string reason)
    {
        EmployeeId = employeeId;
        Reason = reason;
    }
}

public class EmployeeReactivatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeReactivatedEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}

public class EmployeeReturnedFromLeaveEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeReturnedFromLeaveEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}

public class EmployeeSalaryUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public decimal OldSalary { get; }
    public decimal NewSalary { get; }

    public EmployeeSalaryUpdatedEvent(Guid employeeId, decimal oldSalary, decimal newSalary)
    {
        EmployeeId = employeeId;
        OldSalary = oldSalary;
        NewSalary = newSalary;
    }
}

public class EmployeeJobTitleChangedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public string OldTitle { get; }
    public string NewTitle { get; }

    public EmployeeJobTitleChangedEvent(Guid employeeId, string oldTitle, string newTitle)
    {
        EmployeeId = employeeId;
        OldTitle = oldTitle;
        NewTitle = newTitle;
    }
}

public class EmployeeTransferredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid? OldDepartmentId { get; }
    public Guid NewDepartmentId { get; }

    public EmployeeTransferredEvent(Guid employeeId, Guid? oldDepartmentId, Guid newDepartmentId)
    {
        EmployeeId = employeeId;
        OldDepartmentId = oldDepartmentId;
        NewDepartmentId = newDepartmentId;
    }
}

public class EmployeeLeaveBalanceUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public int NewBalance { get; }

    public EmployeeLeaveBalanceUpdatedEvent(Guid employeeId, int newBalance)
    {
        EmployeeId = employeeId;
        NewBalance = newBalance;
    }
}

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

public class LeaveRequestedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public LeaveType LeaveType { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public LeaveRequestedEvent(Guid employeeId, Guid leaveId, LeaveType leaveType, DateTime startDate, DateTime endDate)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
    }
}

public class LeaveApprovedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public Guid ApproverId { get; }

    public LeaveApprovedEvent(Guid employeeId, Guid leaveId, Guid approverId)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        ApproverId = approverId;
    }
}

public class LeaveRejectedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public Guid RejectorId { get; }
    public string Reason { get; }

    public LeaveRejectedEvent(Guid employeeId, Guid leaveId, Guid rejectorId, string reason)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        RejectorId = rejectorId;
        Reason = reason;
    }
}

public class ShiftAssignedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid ShiftId { get; }
    public DateOnly Date { get; }
    public ShiftPattern Pattern { get; }

    public ShiftAssignedEvent(Guid employeeId, Guid shiftId, DateOnly date, ShiftPattern pattern)
    {
        EmployeeId = employeeId;
        ShiftId = shiftId;
        Date = date;
        Pattern = pattern;
    }
}

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

public class EmployeeEnrolledInTrainingEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid TrainingId { get; }
    public string TrainingTitle { get; }

    public EmployeeEnrolledInTrainingEvent(Guid employeeId, Guid trainingId, string trainingTitle)
    {
        EmployeeId = employeeId;
        TrainingId = trainingId;
        TrainingTitle = trainingTitle;
    }
}