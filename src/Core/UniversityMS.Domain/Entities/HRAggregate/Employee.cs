using System.Diagnostics.Contracts;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.HREvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;
using InvalidOperationException = UniversityMS.Domain.Exceptions.InvalidOperationException;

namespace UniversityMS.Domain.Entities.HRAggregate;


/// <summary>
/// Çalışan (Employee) - HR Aggregate Root
/// Hem akademik hem idari personeli kapsar
/// </summary>
public class Employee : AuditableEntity, IAggregateRoot
{
    public EmployeeNumber EmployeeNumber { get; private set; } = null!;
    public AcademicTitle? AcademicTitle { get; private set; }
    public Guid PersonId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public string JobTitle { get; private set; } = null!;
    public EmploymentStatus Status { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? TerminationDate { get; private set; }
    public SalaryInfo Salary { get; private set; } = null!;
    public WorkingHours WorkingHours { get; private set; } = null!;
    public LeaveBalance AnnualLeaveBalance { get; private set; } = null!;
    public string? Notes { get; private set; }

    // Navigation Properties
    public Person Person { get; private set; } = null!;
    public Department? Department { get; private set; }

    // Collections
    private readonly List<Contract> _contracts = new();
    public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

    private readonly List<Leave> _leaves = new();
    public IReadOnlyCollection<Leave> Leaves => _leaves.AsReadOnly();

    private readonly List<Shift> _shifts = new();
    public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();

    private readonly List<PerformanceReview> _performanceReviews = new();
    public IReadOnlyCollection<PerformanceReview> PerformanceReviews => _performanceReviews.AsReadOnly();

    private readonly List<Training> _trainings = new();
    public IReadOnlyCollection<Training> Trainings => _trainings.AsReadOnly();

    // Parameterless constructor for EF Core
    private Employee() { }

    private Employee(
        EmployeeNumber employeeNumber,
        Guid personId,
        string jobTitle,
        DateTime hireDate,
        SalaryInfo salary,
        WorkingHours workingHours,
        Guid? departmentId = null,
        string? notes = null)
    {
        EmployeeNumber = employeeNumber;
        PersonId = personId;
        JobTitle = jobTitle;
        Status = EmploymentStatus.Active;
        HireDate = hireDate;
        Salary = salary;
        WorkingHours = workingHours;
        DepartmentId = departmentId;
        Notes = notes;

        // İlk yıl için standart 14 gün yıllık izin
        AnnualLeaveBalance = LeaveBalance.CreateForNewEmployee();

        AddDomainEvent(new EmployeeHiredEvent(Id, EmployeeNumber, PersonId, HireDate));
    }

    public static Employee Create(
        EmployeeNumber employeeNumber,
        Guid personId,
        string jobTitle,
        DateTime hireDate,
        SalaryInfo salary,
        WorkingHours? workingHours = null,
        Guid? departmentId = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new DomainException("İş ünvanı boş olamaz.");

        if (hireDate > DateTime.UtcNow)
            throw new DomainException("İşe alım tarihi gelecekte olamaz.");

        return new Employee(
            employeeNumber,
            personId,
            jobTitle,
            hireDate,
            salary,
            workingHours ?? WorkingHours.CreateStandard(),
            departmentId,
            notes);
    }

    #region Contract Management

    public void AddContract(Contract contract)
    {
        if (contract.EmployeeId != Id)
            throw new DomainException("Sözleşme bu çalışana ait değil.");

        // Aktif bir sözleşme varsa hata ver
        if (_contracts.Any(c => c.Status == ContractStatus.Active))
            throw new DomainException("Çalışanın zaten aktif bir sözleşmesi var.");

        _contracts.Add(contract);
        AddDomainEvent(new ContractAddedEvent(Id, contract.Id));
    }

    public Contract? GetActiveContract()
    {
        return _contracts.FirstOrDefault(c => c.Status == ContractStatus.Active);
    }

    public bool HasActiveContract()
    {
        return _contracts.Any(c => c.Status == ContractStatus.Active);
    }

    #endregion

    #region Leave Management
    public void RequestLeave(Leave leave)
    {
        if (leave.EmployeeId != Id)
            throw new DomainException("İzin talebi bu çalışana ait değil.");

        if (Status != EmploymentStatus.Active)
            throw new DomainException("Sadece aktif çalışanlar izin talep edebilir.");

        // ✅ DÜZELTILMIŞ: Basit kontrolü kullan
        var durationDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;

        if (!AnnualLeaveBalance.CanTakeLeave(durationDays))
        {
            var remaining = AnnualLeaveBalance.GetRemainingDays();
            throw new DomainException(
                $"Yetersiz izin bakiyesi. Kalan: {remaining} gün");
        }

        _leaves.Add(leave);

        // ✅ DÜZELTILMIŞ: UseLeave() çağır
        AnnualLeaveBalance = AnnualLeaveBalance.UseLeave(durationDays);

        AddDomainEvent(new LeaveRequestedEvent(
            Id,
            leave.Id,
            leave.LeaveType,
            leave.StartDate,
            leave.EndDate));
    }

    public void RejectLeave(Guid leaveId, Guid rejectorId, string reason)
    {
        var leave = _leaves.FirstOrDefault(l => l.Id == leaveId);
        if (leave == null)
            throw new DomainException("İzin bulunamadı.");

        leave.Reject(rejectorId, reason);

        // ✅ DÜZELTILMIŞ: RefundLeave() çağır
        var durationDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
        AnnualLeaveBalance = AnnualLeaveBalance.RefundLeave(durationDays);

        AddDomainEvent(new LeaveRejectedEvent(Id, leaveId, rejectorId, reason));
    }

    public void RefreshAnnualLeave()
    {
        AnnualLeaveBalance = AnnualLeaveBalance.RefreshAnnualLeave(14);
        AddDomainEvent(new EmployeeAnnualLeaveRefreshedEvent(Id));
    }
    public void ApproveLeave(Guid leaveId, Guid approverId)
    {
        var leave = _leaves.FirstOrDefault(l => l.Id == leaveId);
        if (leave == null)
            throw new DomainException("İzin bulunamadı.");

        leave.Approve(approverId);

        // Status güncelle
        if (leave.IsCurrentlyOnLeave())
        {
            Status = EmploymentStatus.OnLeave;
        }

        AddDomainEvent(new LeaveApprovedEvent(Id, leaveId, approverId));
    }

    public void ReturnFromLeave()
    {
        if (Status != EmploymentStatus.OnLeave)
            throw new DomainException("Çalışan izinli değil.");

        Status = EmploymentStatus.Active;
        AddDomainEvent(new EmployeeReturnedFromLeaveEvent(Id));
    }


    #endregion

    #region Shift Management

    public void AssignShift(Shift shift)
    {
        if (shift.EmployeeId != Id)
            throw new DomainException("Vardiya bu çalışana ait değil.");

        if (Status != EmploymentStatus.Active)
            throw new DomainException("Sadece aktif çalışanlara vardiya atanabilir.");

        // Çakışan vardiya kontrolü
        var hasConflict = _shifts.Any(s =>
            s.Status == ShiftStatus.Scheduled &&
            s.Date == shift.Date &&
            ((shift.StartTime >= s.StartTime && shift.StartTime < s.EndTime) ||
             (shift.EndTime > s.StartTime && shift.EndTime <= s.EndTime)));

        if (hasConflict)
            throw new DomainException("Bu tarihte çakışan bir vardiya var.");

        _shifts.Add(shift);
        AddDomainEvent(new ShiftAssignedEvent(Id, shift.Id, shift.Date, shift.ShiftPattern));
    }

    #endregion

    #region Performance Management

    public void AddPerformanceReview(PerformanceReview review)
    {
        if (review.EmployeeId != Id)
            throw new DomainException("Performans değerlendirmesi bu çalışana ait değil.");

        _performanceReviews.Add(review);
        AddDomainEvent(new PerformanceReviewAddedEvent(Id, review.Id, review.ReviewPeriod));
    }

    public PerformanceReview? GetLatestPerformanceReview()
    {
        return _performanceReviews
            .Where(pr => pr.Status == PerformanceReviewStatus.Completed)
            .OrderByDescending(pr => pr.ReviewDate)
            .FirstOrDefault();
    }

    public decimal GetAveragePerformanceScore()
    {
        var completedReviews = _performanceReviews
            .Where(pr => pr.Status == PerformanceReviewStatus.Completed)
            .ToList();

        if (!completedReviews.Any())
            return 0;

        return completedReviews.Average(pr => pr.OverallScore.Score);
    }

    #endregion

    #region Training Management

    public void EnrollInTraining(Training training)
    {
        if (Status != EmploymentStatus.Active)
            throw new DomainException("Sadece aktif çalışanlar eğitime katılabilir.");

        var isAlreadyEnrolled = training.IsEmployeeEnrolled(Id);
        if (isAlreadyEnrolled)
            throw new DomainException("Çalışan bu eğitime zaten kayıtlı.");

        training.EnrollEmployee(Id);
        _trainings.Add(training);

        AddDomainEvent(new EmployeeEnrolledInTrainingEvent(Id, training.Id, training.Title));
    }

    public int GetTotalTrainingHours()
    {
        return _trainings
            .Where(t => t.Status == TrainingStatus.Completed)
            .Sum(t => t.Duration.Hours);
    }

    #endregion

    #region Employment Management

    public void UpdateSalary(SalaryInfo newSalary)
    {
        var oldSalary = Salary;
        Salary = newSalary;
        AddDomainEvent(new EmployeeSalaryUpdatedEvent(Id, oldSalary.GetGrossSalary(), newSalary.GetGrossSalary()));
    }

    public void UpdateAcademicTitle(AcademicTitle academicTitle)
    {
        var oldAcademicTitle = AcademicTitle;
        AcademicTitle = academicTitle;
        AddDomainEvent(new EmployeeAcademicTitleUpdatedEvent(Id, oldAcademicTitle, academicTitle));
    }

    public void UpdateJobTitle(string newJobTitle)
    {
        if (string.IsNullOrWhiteSpace(newJobTitle))
            throw new DomainException("İş ünvanı boş olamaz.");

        var oldTitle = JobTitle;
        JobTitle = newJobTitle;
        AddDomainEvent(new EmployeeJobTitleChangedEvent(Id, oldTitle, newJobTitle));
    }

    public void TransferToDepartment(Guid newDepartmentId)
    {
        var oldDepartmentId = DepartmentId;
        DepartmentId = newDepartmentId;
        AddDomainEvent(new EmployeeTransferredEvent(Id, oldDepartmentId, newDepartmentId));
    }

    public void Suspend(string reason)
    {
        if (Status == EmploymentStatus.Suspended)
            throw new DomainException("Çalışan zaten işten uzaklaştırılmış.");

        Status = EmploymentStatus.Suspended;
        AddDomainEvent(new EmployeeSuspendedEvent(Id, reason));
    }

    public void Reactivate()
    {
        if (Status == EmploymentStatus.Active)
            throw new DomainException("Çalışan zaten aktif.");

        if (Status == EmploymentStatus.Terminated || Status == EmploymentStatus.Resigned)
            throw new DomainException("Sonlandırılmış çalışan tekrar aktif edilemez.");

        Status = EmploymentStatus.Active;
        AddDomainEvent(new EmployeeReactivatedEvent(Id));
    }

    public void Terminate(DateTime terminationDate, string reason)
    {
        if (Status == EmploymentStatus.Terminated)
            throw new DomainException("Çalışan zaten işten çıkarılmış.");

        if (terminationDate < HireDate)
            throw new DomainException("İşten çıkış tarihi işe alım tarihinden önce olamaz.");

        Status = EmploymentStatus.Terminated;
        TerminationDate = terminationDate;

        // Aktif sözleşmeyi sonlandır
        var activeContract = GetActiveContract();
        activeContract?.Terminate(terminationDate, reason);

        AddDomainEvent(new EmployeeTerminatedEvent(Id, terminationDate, reason));
    }

    public void Resign(DateTime resignationDate)
    {
        if (Status == EmploymentStatus.Resigned)
            throw new DomainException("Çalışan zaten istifa etmiş.");

        if (resignationDate < HireDate)
            throw new DomainException("İstifa tarihi işe alım tarihinden önce olamaz.");

        Status = EmploymentStatus.Resigned;
        TerminationDate = resignationDate;

        AddDomainEvent(new EmployeeResignedEvent(Id, resignationDate));
    }

    public void Retire(DateTime retirementDate)
    {
        if (Status == EmploymentStatus.Retired)
            throw new DomainException("Çalışan zaten emekli.");

        if (retirementDate < HireDate)
            throw new DomainException("Emeklilik tarihi işe alım tarihinden önce olamaz.");

        Status = EmploymentStatus.Retired;
        TerminationDate = retirementDate;

        AddDomainEvent(new EmployeeRetiredEvent(Id, retirementDate));
    }

    #endregion

    #region Helper Methods

    public bool IsActive() => Status == EmploymentStatus.Active;

    public int GetTenureInYears()
    {
        var endDate = TerminationDate ?? DateTime.UtcNow;
        return (int)((endDate - HireDate).TotalDays / 365.25);
    }

    public int GetTenureInMonths()
    {
        var endDate = TerminationDate ?? DateTime.UtcNow;
        return (int)((endDate - HireDate).TotalDays / 30.44);
    }

    public void UpdateAnnualLeaveBalance(int totalDays)
    {
        if (totalDays < 0)
            throw new DomainException("İzin günü negatif olamaz.");

        var currentUsed = AnnualLeaveBalance.UsedDays;
        AnnualLeaveBalance = LeaveBalance.Create(totalDays, currentUsed);

        AddDomainEvent(new EmployeeLeaveBalanceUpdatedEvent(Id, totalDays));
    }

    #endregion


    public void UpdateDepartment(Guid departmentId)
    {
        DepartmentId = departmentId;
    }


    public void UpdateWorkingHours(WorkingHours workingHours)
    {
        if (workingHours == null)
            throw new ArgumentNullException(nameof(workingHours));

        WorkingHours = workingHours;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    public void Terminate(DateTime terminationDate)
    {
        if (terminationDate < HireDate)
            throw new InvalidOperationException("İşten ayrılış tarihi işe alım tarihinden önce olamaz");

        TerminationDate = terminationDate;
        Status = EmploymentStatus.Terminated;
    }

    // Eğer EmployeeNumber value object ise:
    public string GetEmployeeNumber() => EmployeeNumber.Value;

}