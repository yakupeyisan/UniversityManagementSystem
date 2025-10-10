using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;


// Employee Events
public class EmployeeHiredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public string EmployeeNumber { get; }
    public Guid PersonId { get; }
    public DateTime HireDate { get; }

    public EmployeeHiredEvent(Guid employeeId, string employeeNumber, Guid personId, DateTime hireDate)
    {
        EmployeeId = employeeId;
        EmployeeNumber = employeeNumber;
        PersonId = personId;
        HireDate = hireDate;
    }
}

public class EmployeeSalaryUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public decimal OldGrossSalary { get; }
    public decimal NewGrossSalary { get; }

    public EmployeeSalaryUpdatedEvent(Guid employeeId, decimal oldGrossSalary, decimal newGrossSalary)
    {
        EmployeeId = employeeId;
        OldGrossSalary = oldGrossSalary;
        NewGrossSalary = newGrossSalary;
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

public class EmployeeLeaveBalanceUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public int NewTotalDays { get; }

    public EmployeeLeaveBalanceUpdatedEvent(Guid employeeId, int newTotalDays)
    {
        EmployeeId = employeeId;
        NewTotalDays = newTotalDays;
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

// Contract Events
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

public class ContractSignedEvent : BaseDomainEvent
{
    public Guid ContractId { get; }
    public Guid SignedByEmployeeId { get; }
    public DateTime SignedDate { get; }

    public ContractSignedEvent(Guid contractId, Guid signedByEmployeeId, DateTime signedDate)
    {
        ContractId = contractId;
        SignedByEmployeeId = signedByEmployeeId;
        SignedDate = signedDate;
    }
}

public class ContractRenewedEvent : BaseDomainEvent
{
    public Guid ContractId { get; }
    public DateTime NewEndDate { get; }
    public decimal? NewSalary { get; }

    public ContractRenewedEvent(Guid contractId, DateTime newEndDate, decimal? newSalary)
    {
        ContractId = contractId;
        NewEndDate = newEndDate;
        NewSalary = newSalary;
    }
}

public class ContractExpiringSoonEvent : BaseDomainEvent
{
    public Guid ContractId { get; }
    public Guid EmployeeId { get; }
    public DateTime ExpirationDate { get; }
    public int DaysRemaining { get; }

    public ContractExpiringSoonEvent(Guid contractId, Guid employeeId, DateTime expirationDate, int daysRemaining)
    {
        ContractId = contractId;
        EmployeeId = employeeId;
        ExpirationDate = expirationDate;
        DaysRemaining = daysRemaining;
    }
}

public class ContractExpiredEvent : BaseDomainEvent
{
    public Guid ContractId { get; }
    public Guid EmployeeId { get; }
    public DateTime ExpirationDate { get; }

    public ContractExpiredEvent(Guid contractId, Guid employeeId, DateTime expirationDate)
    {
        ContractId = contractId;
        EmployeeId = employeeId;
        ExpirationDate = expirationDate;
    }
}

public class ContractTerminatedEvent : BaseDomainEvent
{
    public Guid ContractId { get; }
    public Guid EmployeeId { get; }
    public DateTime TerminationDate { get; }
    public string Reason { get; }

    public ContractTerminatedEvent(Guid contractId, Guid employeeId, DateTime terminationDate, string reason)
    {
        ContractId = contractId;
        EmployeeId = employeeId;
        TerminationDate = terminationDate;
        Reason = reason;
    }
}

// Leave Events
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

public class LeaveCancelledEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }

    public LeaveCancelledEvent(Guid employeeId, Guid leaveId)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
    }
}

public class LeaveCompletedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }

    public LeaveCompletedEvent(Guid employeeId, Guid leaveId)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
    }
}

// Shift Events
public class ShiftAssignedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid ShiftId { get; }
    public DateOnly Date { get; }
    public ShiftPattern ShiftPattern { get; }

    public ShiftAssignedEvent(Guid employeeId, Guid shiftId, DateOnly date, ShiftPattern shiftPattern)
    {
        EmployeeId = employeeId;
        ShiftId = shiftId;
        Date = date;
        ShiftPattern = shiftPattern;
    }
}

public class ShiftStartedEvent : BaseDomainEvent
{
    public Guid ShiftId { get; }
    public Guid EmployeeId { get; }
    public DateTime StartTime { get; }

    public ShiftStartedEvent(Guid shiftId, Guid employeeId, DateTime startTime)
    {
        ShiftId = shiftId;
        EmployeeId = employeeId;
        StartTime = startTime;
    }
}

public class ShiftCompletedEvent : BaseDomainEvent
{
    public Guid ShiftId { get; }
    public Guid EmployeeId { get; }
    public DateTime EndTime { get; }
    public decimal? OvertimeHours { get; }

    public ShiftCompletedEvent(Guid shiftId, Guid employeeId, DateTime endTime, decimal? overtimeHours)
    {
        ShiftId = shiftId;
        EmployeeId = employeeId;
        EndTime = endTime;
        OvertimeHours = overtimeHours;
    }
}

public class ShiftCancelledEvent : BaseDomainEvent
{
    public Guid ShiftId { get; }
    public Guid EmployeeId { get; }
    public string Reason { get; }

    public ShiftCancelledEvent(Guid shiftId, Guid employeeId, string reason)
    {
        ShiftId = shiftId;
        EmployeeId = employeeId;
        Reason = reason;
    }
}

public class ShiftModifiedEvent : BaseDomainEvent
{
    public Guid ShiftId { get; }
    public Guid EmployeeId { get; }
    public TimeOnly NewStartTime { get; }
    public TimeOnly NewEndTime { get; }

    public ShiftModifiedEvent(Guid shiftId, Guid employeeId, TimeOnly newStartTime, TimeOnly newEndTime)
    {
        ShiftId = shiftId;
        EmployeeId = employeeId;
        NewStartTime = newStartTime;
        NewEndTime = newEndTime;
    }
}

// Performance Review Events
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

public class PerformanceReviewStartedEvent : BaseDomainEvent
{
    public Guid ReviewId { get; }
    public Guid EmployeeId { get; }
    public Guid ReviewerId { get; }

    public PerformanceReviewStartedEvent(Guid reviewId, Guid employeeId, Guid reviewerId)
    {
        ReviewId = reviewId;
        EmployeeId = employeeId;
        ReviewerId = reviewerId;
    }
}

public class PerformanceReviewCompletedEvent : BaseDomainEvent
{
    public Guid ReviewId { get; }
    public Guid EmployeeId { get; }
    public decimal OverallScore { get; }
    public PerformanceRating OverallRating { get; }

    public PerformanceReviewCompletedEvent(Guid reviewId, Guid employeeId, decimal overallScore, PerformanceRating overallRating)
    {
        ReviewId = reviewId;
        EmployeeId = employeeId;
        OverallScore = overallScore;
        OverallRating = overallRating;
    }
}

public class PerformanceReviewApprovedEvent : BaseDomainEvent
{
    public Guid ReviewId { get; }
    public Guid EmployeeId { get; }
    public Guid ApproverId { get; }

    public PerformanceReviewApprovedEvent(Guid reviewId, Guid employeeId, Guid approverId)
    {
        ReviewId = reviewId;
        EmployeeId = employeeId;
        ApproverId = approverId;
    }
}

// Training Events
public class TrainingCreatedEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public string Title { get; }
    public TrainingType Type { get; }
    public DateTime StartDate { get; }

    public TrainingCreatedEvent(Guid trainingId, string title, TrainingType type, DateTime startDate)
    {
        TrainingId = trainingId;
        Title = title;
        Type = type;
        StartDate = startDate;
    }
}

public class TrainingRegistrationOpenedEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public string Title { get; }
    public int MaxParticipants { get; }

    public TrainingRegistrationOpenedEvent(Guid trainingId, string title, int maxParticipants)
    {
        TrainingId = trainingId;
        Title = title;
        MaxParticipants = maxParticipants;
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

public class EmployeeUnenrolledFromTrainingEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid TrainingId { get; }

    public EmployeeUnenrolledFromTrainingEvent(Guid employeeId, Guid trainingId)
    {
        EmployeeId = employeeId;
        TrainingId = trainingId;
    }
}

public class TrainingStartedEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public string Title { get; }
    public int EnrolledCount { get; }

    public TrainingStartedEvent(Guid trainingId, string title, int enrolledCount)
    {
        TrainingId = trainingId;
        Title = title;
        EnrolledCount = enrolledCount;
    }
}

public class TrainingCompletedEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public string Title { get; }
    public int CompletedCount { get; }
    public decimal CompletionRate { get; }

    public TrainingCompletedEvent(Guid trainingId, string title, int completedCount, decimal completionRate)
    {
        TrainingId = trainingId;
        Title = title;
        CompletedCount = completedCount;
        CompletionRate = completionRate;
    }
}

public class TrainingCancelledEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public string Title { get; }

    public TrainingCancelledEvent(Guid trainingId, string title)
    {
        TrainingId = trainingId;
        Title = title;
    }
}

public class TrainingPostponedEvent : BaseDomainEvent
{
    public Guid TrainingId { get; }
    public DateTime OldStartDate { get; }
    public DateTime NewStartDate { get; }

    public TrainingPostponedEvent(Guid trainingId, DateTime oldStartDate, DateTime newStartDate)
    {
        TrainingId = trainingId;
        OldStartDate = oldStartDate;
        NewStartDate = newStartDate;
    }
}

public class TrainingEnrollmentCompletedEvent : BaseDomainEvent
{
    public Guid EnrollmentId { get; }
    public Guid TrainingId { get; }
    public Guid EmployeeId { get; }
    public decimal? Score { get; }
    public string? Certificate { get; }

    public TrainingEnrollmentCompletedEvent(Guid enrollmentId, Guid trainingId, Guid employeeId, decimal? score, string? certificate)
    {
        EnrollmentId = enrollmentId;
        TrainingId = trainingId;
        EmployeeId = employeeId;
        Score = score;
        Certificate = certificate;
    }
}