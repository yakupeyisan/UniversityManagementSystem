using System.Diagnostics.Contracts;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.HREvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.HRAggregate;


/// <summary>
/// Çalışan (Employee) - HR Aggregate Root
/// Hem akademik hem idari personeli kapsar
/// </summary>
public class Employee : AuditableEntity, IAggregateRoot
{
    public EmployeeNumber EmployeeNumber { get; private set; } = null!;
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
        AnnualLeaveBalance = LeaveBalance.Create(14);

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

        // Yıllık izin kontrolü
        if (leave.LeaveType == LeaveType.Annual)
        {
            var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
            if (!AnnualLeaveBalance.CanTakeLeave(totalDays))
                throw new DomainException($"Yetersiz izin bakiyesi. Kalan: {AnnualLeaveBalance.RemainingDays} gün");
        }

        _leaves.Add(leave);
        AddDomainEvent(new LeaveRequestedEvent(Id, leave.Id, leave.LeaveType, leave.StartDate, leave.EndDate));
    }

    public void ApproveLeave(Guid leaveId, Guid approverId)
    {
        var leave = _leaves.FirstOrDefault(l => l.Id == leaveId);
        if (leave == null)
            throw new DomainException("İzin bulunamadı.");

        leave.Approve(approverId);

        // Yıllık izin ise bakiyeden düş
        if (leave.LeaveType == LeaveType.Annual)
        {
            var totalDays = (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
            AnnualLeaveBalance = AnnualLeaveBalance.UseLeave(totalDays);
        }

        // İzinli duruma al
        if (leave.IsCurrentlyOnLeave())
        {
            Status = EmploymentStatus.OnLeave;
        }

        AddDomainEvent(new LeaveApprovedEvent(Id, leaveId, approverId));
    }

    public void RejectLeave(Guid leaveId, Guid rejectorId, string reason)
    {
        var leave = _leaves.FirstOrDefault(l => l.Id == leaveId);
        if (leave == null)
            throw new DomainException("İzin bulunamadı.");

        leave.Reject(rejectorId, reason);
        AddDomainEvent(new LeaveRejectedEvent(Id, leaveId, rejectorId, reason));
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
}

/// <summary>
/// İş Sözleşmesi Entity
/// </summary>
public class Contract : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public string ContractNumber { get; private set; } = null!;
    public ContractType Type { get; private set; }
    public ContractStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public SalaryInfo Salary { get; private set; } = null!;
    public WorkingHours WorkingHours { get; private set; } = null!;
    public string? Terms { get; private set; }
    public string? FilePath { get; private set; }
    public Guid? SignedByEmployeeId { get; private set; }
    public DateTime? SignedDate { get; private set; }
    public string? TerminationReason { get; private set; }

    // Navigation Property
    public Employee Employee { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Contract() { }

    private Contract(
        Guid employeeId,
        string contractNumber,
        ContractType type,
        DateTime startDate,
        DateTime? endDate,
        SalaryInfo salary,
        WorkingHours workingHours,
        string? terms = null)
    {
        EmployeeId = employeeId;
        ContractNumber = contractNumber;
        Type = type;
        Status = ContractStatus.Draft;
        StartDate = startDate;
        EndDate = endDate;
        Salary = salary;
        WorkingHours = workingHours;
        Terms = terms;
    }

    public static Contract Create(
        Guid employeeId,
        string contractNumber,
        ContractType type,
        DateTime startDate,
        SalaryInfo salary,
        WorkingHours workingHours,
        DateTime? endDate = null,
        string? terms = null)
    {
        if (string.IsNullOrWhiteSpace(contractNumber))
            throw new DomainException("Sözleşme numarası boş olamaz.");

        if (startDate < DateTime.UtcNow.Date)
            throw new DomainException("Sözleşme başlangıç tarihi geçmiş olamaz.");

        // Belirli süreli sözleşmeler için bitiş tarihi zorunlu
        if (type == ContractType.FixedTerm && !endDate.HasValue)
            throw new DomainException("Belirli süreli sözleşmeler için bitiş tarihi belirtilmelidir.");

        // Bitiş tarihi varsa başlangıçtan sonra olmalı
        if (endDate.HasValue && endDate.Value <= startDate)
            throw new DomainException("Sözleşme bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        return new Contract(employeeId, contractNumber, type, startDate, endDate, salary, workingHours, terms);
    }

    public void Activate()
    {
        if (Status == ContractStatus.Active)
            throw new DomainException("Sözleşme zaten aktif.");

        if (Status != ContractStatus.Draft && Status != ContractStatus.PendingRenewal)
            throw new DomainException("Sadece taslak veya yenileme bekleyen sözleşmeler aktif edilebilir.");

        Status = ContractStatus.Active;
    }

    public void Sign(Guid signedByEmployeeId)
    {
        if (Status != ContractStatus.Draft)
            throw new DomainException("Sadece taslak sözleşmeler imzalanabilir.");

        SignedByEmployeeId = signedByEmployeeId;
        SignedDate = DateTime.UtcNow;
        Status = ContractStatus.Active;
    }

    public void Renew(DateTime newEndDate, SalaryInfo? newSalary = null)
    {
        if (Status != ContractStatus.Active && Status != ContractStatus.PendingRenewal)
            throw new DomainException("Sadece aktif veya yenileme bekleyen sözleşmeler yenilenebilir.");

        if (newEndDate <= (EndDate ?? StartDate))
            throw new DomainException("Yeni bitiş tarihi mevcut tarihten sonra olmalıdır.");

        EndDate = newEndDate;

        if (newSalary != null)
            Salary = newSalary;

        Status = ContractStatus.Renewed;
    }

    public void MarkForRenewal()
    {
        if (Status != ContractStatus.Active)
            throw new DomainException("Sadece aktif sözleşmeler yenileme için işaretlenebilir.");

        if (!EndDate.HasValue)
            throw new DomainException("Belirsiz süreli sözleşmeler yenilenemez.");

        Status = ContractStatus.PendingRenewal;
    }

    public void Expire()
    {
        if (!EndDate.HasValue)
            throw new DomainException("Belirsiz süreli sözleşmeler sona eremez.");

        if (DateTime.UtcNow < EndDate.Value)
            throw new DomainException("Sözleşme bitiş tarihine ulaşmamış.");

        Status = ContractStatus.Expired;
    }

    public void Terminate(DateTime terminationDate, string reason)
    {
        if (Status == ContractStatus.Terminated)
            throw new DomainException("Sözleşme zaten feshedilmiş.");

        if (Status != ContractStatus.Active)
            throw new DomainException("Sadece aktif sözleşmeler feshedilebilir.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Fesih sebebi belirtilmelidir.");

        Status = ContractStatus.Terminated;
        EndDate = terminationDate;
        TerminationReason = reason;
    }

    public void Cancel()
    {
        if (Status != ContractStatus.Draft)
            throw new DomainException("Sadece taslak sözleşmeler iptal edilebilir.");

        Status = ContractStatus.Cancelled;
    }

    public void UpdateSalary(SalaryInfo newSalary)
    {
        if (Status != ContractStatus.Active)
            throw new DomainException("Sadece aktif sözleşmelerde maaş güncellenebilir.");

        Salary = newSalary;
    }

    public void AttachFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("Dosya yolu boş olamaz.");

        FilePath = filePath;
    }

    public bool IsExpiringSoon(int daysThreshold = 30)
    {
        if (!EndDate.HasValue)
            return false;

        var daysUntilExpiry = (EndDate.Value - DateTime.UtcNow).Days;
        return daysUntilExpiry > 0 && daysUntilExpiry <= daysThreshold;
    }

    public bool IsExpired()
    {
        return EndDate.HasValue && DateTime.UtcNow >= EndDate.Value;
    }

    public int GetDurationInMonths()
    {
        var endDate = EndDate ?? DateTime.UtcNow;
        return (int)((endDate - StartDate).TotalDays / 30.44);
    }

    public int GetRemainingDays()
    {
        if (!EndDate.HasValue)
            return int.MaxValue; // Belirsiz süreli

        var remaining = (EndDate.Value - DateTime.UtcNow).Days;
        return Math.Max(0, remaining);
    }
}

/// <summary>
/// Vardiya Entity
/// </summary>
public class Shift : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public ShiftPattern ShiftPattern { get; private set; }
    public ShiftStatus Status { get; private set; }
    public decimal? OvertimeHours { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Property
    public Employee Employee { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Shift() { }

    private Shift(
        Guid employeeId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        ShiftPattern shiftPattern,
        string? notes = null)
    {
        EmployeeId = employeeId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        ShiftPattern = shiftPattern;
        Status = ShiftStatus.Scheduled;
        Notes = notes;
    }

    public static Shift Create(
        Guid employeeId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        ShiftPattern shiftPattern,
        string? notes = null)
    {
        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Vardiya tarihi geçmiş olamaz.");

        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden büyük olmalıdır.");

        return new Shift(employeeId, date, startTime, endTime, shiftPattern, notes);
    }

    public void Start()
    {
        if (Status != ShiftStatus.Scheduled)
            throw new DomainException("Sadece planlanmış vardiyalar başlatılabilir.");

        if (Date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Gelecek tarihli vardiya başlatılamaz.");

        Status = ShiftStatus.InProgress;
    }

    public void Complete(decimal? overtimeHours = null)
    {
        if (Status != ShiftStatus.InProgress)
            throw new DomainException("Sadece devam eden vardiyalar tamamlanabilir.");

        if (overtimeHours.HasValue && overtimeHours < 0)
            throw new DomainException("Fazla mesai saati negatif olamaz.");

        Status = ShiftStatus.Completed;
        OvertimeHours = overtimeHours;
    }

    public void Cancel(string reason)
    {
        if (Status == ShiftStatus.Completed)
            throw new DomainException("Tamamlanmış vardiya iptal edilemez.");

        Status = ShiftStatus.Cancelled;
        Notes = $"İptal sebebi: {reason}";
    }

    public void Modify(TimeOnly newStartTime, TimeOnly newEndTime)
    {
        if (Status != ShiftStatus.Scheduled)
            throw new DomainException("Sadece planlanmış vardiyalar değiştirilebilir.");

        if (newEndTime <= newStartTime)
            throw new DomainException("Bitiş saati başlangıç saatinden büyük olmalıdır.");

        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = ShiftStatus.Modified;
    }

    public decimal GetTotalHours()
    {
        return (decimal)(EndTime - StartTime).TotalHours;
    }

    public decimal GetTotalHoursWithOvertime()
    {
        return GetTotalHours() + (OvertimeHours ?? 0);
    }

    public bool IsNightShift()
    {
        return ShiftPattern == ShiftPattern.Night ||
               StartTime.Hour >= 22 ||
               EndTime.Hour <= 6;
    }
}

/// <summary>
/// Performans Değerlendirme Entity
/// </summary>
public class PerformanceReview : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public string ReviewPeriod { get; private set; } = null!; // e.g., "2024-Q1", "2024-Annual"
    public DateTime ReviewDate { get; private set; }
    public PerformanceReviewStatus Status { get; private set; }
    public PerformanceScore OverallScore { get; private set; } = null!;
    public PerformanceRating OverallRating { get; private set; }

    // Değerlendirme kriterleri skorları
    public decimal QualityOfWorkScore { get; private set; }
    public decimal ProductivityScore { get; private set; }
    public decimal TeamworkScore { get; private set; }
    public decimal CommunicationScore { get; private set; }
    public decimal LeadershipScore { get; private set; }

    public string? Strengths { get; private set; }
    public string? AreasForImprovement { get; private set; }
    public string? Goals { get; private set; }
    public string? ReviewerComments { get; private set; }
    public string? EmployeeComments { get; private set; }

    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }

    // Navigation Properties
    public Employee Employee { get; private set; } = null!;
    public Employee Reviewer { get; private set; } = null!;
    public Employee? Approver { get; private set; }

    // Parameterless constructor for EF Core
    private PerformanceReview() { }

    private PerformanceReview(
        Guid employeeId,
        Guid reviewerId,
        string reviewPeriod,
        DateTime reviewDate)
    {
        EmployeeId = employeeId;
        ReviewerId = reviewerId;
        ReviewPeriod = reviewPeriod;
        ReviewDate = reviewDate;
        Status = PerformanceReviewStatus.Scheduled;
    }

    public static PerformanceReview Create(
        Guid employeeId,
        Guid reviewerId,
        string reviewPeriod,
        DateTime reviewDate)
    {
        if (string.IsNullOrWhiteSpace(reviewPeriod))
            throw new DomainException("Değerlendirme dönemi belirtilmelidir.");

        if (reviewDate > DateTime.UtcNow)
            throw new DomainException("Değerlendirme tarihi gelecekte olamaz.");

        return new PerformanceReview(employeeId, reviewerId, reviewPeriod, reviewDate);
    }

    public void StartReview()
    {
        if (Status != PerformanceReviewStatus.Scheduled)
            throw new DomainException("Sadece planlanmış değerlendirmeler başlatılabilir.");

        Status = PerformanceReviewStatus.InProgress;
    }

    public void CompleteReview(
        decimal qualityOfWorkScore,
        decimal productivityScore,
        decimal teamworkScore,
        decimal communicationScore,
        decimal leadershipScore,
        string? strengths = null,
        string? areasForImprovement = null,
        string? goals = null,
        string? reviewerComments = null)
    {
        if (Status != PerformanceReviewStatus.InProgress)
            throw new DomainException("Sadece devam eden değerlendirmeler tamamlanabilir.");

        ValidateScore(qualityOfWorkScore, nameof(qualityOfWorkScore));
        ValidateScore(productivityScore, nameof(productivityScore));
        ValidateScore(teamworkScore, nameof(teamworkScore));
        ValidateScore(communicationScore, nameof(communicationScore));
        ValidateScore(leadershipScore, nameof(leadershipScore));

        QualityOfWorkScore = qualityOfWorkScore;
        ProductivityScore = productivityScore;
        TeamworkScore = teamworkScore;
        CommunicationScore = communicationScore;
        LeadershipScore = leadershipScore;

        // Genel skoru hesapla (ağırlıklı ortalama)
        var overallScoreValue = (qualityOfWorkScore * 0.3m) +
                               (productivityScore * 0.25m) +
                               (teamworkScore * 0.2m) +
                               (communicationScore * 0.15m) +
                               (leadershipScore * 0.1m);

        OverallScore = PerformanceScore.Create(overallScoreValue);
        OverallRating = OverallScore.GetRating();

        Strengths = strengths;
        AreasForImprovement = areasForImprovement;
        Goals = goals;
        ReviewerComments = reviewerComments;

        Status = PerformanceReviewStatus.Completed;
    }

    public void AddEmployeeComments(string comments)
    {
        if (Status != PerformanceReviewStatus.Completed)
            throw new DomainException("Sadece tamamlanmış değerlendirmelere çalışan yorumu eklenebilir.");

        EmployeeComments = comments;
        Status = PerformanceReviewStatus.PendingApproval;
    }

    public void Approve(Guid approverId)
    {
        if (Status != PerformanceReviewStatus.PendingApproval && Status != PerformanceReviewStatus.Completed)
            throw new DomainException("Değerlendirme onaylanamaz.");

        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;
        Status = PerformanceReviewStatus.Approved;
    }

    public void Cancel()
    {
        if (Status == PerformanceReviewStatus.Approved)
            throw new DomainException("Onaylı değerlendirme iptal edilemez.");

        Status = PerformanceReviewStatus.Cancelled;
    }

    private void ValidateScore(decimal score, string scoreName)
    {
        if (score < 0 || score > 100)
            throw new DomainException($"{scoreName} 0-100 arasında olmalıdır.");
    }

    public bool IsExcellent() => OverallScore >= 90;
    public bool IsGood() => OverallScore >= 70;
    public bool NeedsImprovement() => OverallScore < 60;
}

/// <summary>
/// Eğitim Entity
/// </summary>
public class Training : AuditableEntity
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TrainingType Type { get; private set; }
    public TrainingStatus Status { get; private set; }
    public TrainingDuration Duration { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MaxParticipants { get; private set; }
    public string Instructor { get; private set; } = null!;
    public string? Location { get; private set; }
    public decimal? Cost { get; private set; }
    public string? Materials { get; private set; }
    public bool IsCertified { get; private set; }

    // Kayıtlı çalışanlar
    private readonly List<TrainingEnrollment> _enrollments = new();
    public IReadOnlyCollection<TrainingEnrollment> Enrollments => _enrollments.AsReadOnly();

    // Parameterless constructor for EF Core
    private Training() { }

    private Training(
        string title,
        string description,
        TrainingType type,
        TrainingDuration duration,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        string instructor,
        string? location = null,
        decimal? cost = null,
        bool isCertified = false)
    {
        Title = title;
        Description = description;
        Type = type;
        Status = TrainingStatus.Planned;
        Duration = duration;
        StartDate = startDate;
        EndDate = endDate;
        MaxParticipants = maxParticipants;
        Instructor = instructor;
        Location = location;
        Cost = cost;
        IsCertified = isCertified;
    }

    public static Training Create(
        string title,
        string description,
        TrainingType type,
        TrainingDuration duration,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        string instructor,
        string? location = null,
        decimal? cost = null,
        bool isCertified = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Eğitim başlığı boş olamaz.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Eğitim açıklaması boş olamaz.");

        if (string.IsNullOrWhiteSpace(instructor))
            throw new DomainException("Eğitmen belirtilmelidir.");

        if (startDate >= endDate)
            throw new DomainException("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        if (maxParticipants <= 0)
            throw new DomainException("Maksimum katılımcı sayısı pozitif olmalıdır.");

        return new Training(title, description, type, duration, startDate, endDate,
            maxParticipants, instructor, location, cost, isCertified);
    }

    public void OpenRegistration()
    {
        if (Status != TrainingStatus.Planned)
            throw new DomainException("Sadece planlanmış eğitimler için kayıt açılabilir.");

        Status = TrainingStatus.RegistrationOpen;
    }

    public void EnrollEmployee(Guid employeeId)
    {
        if (Status != TrainingStatus.RegistrationOpen)
            throw new DomainException("Kayıtlar açık değil.");

        if (_enrollments.Count >= MaxParticipants)
            throw new DomainException("Eğitim dolu.");

        if (_enrollments.Any(e => e.EmployeeId == employeeId))
            throw new DomainException("Çalışan zaten kayıtlı.");

        var enrollment = TrainingEnrollment.Create(Id, employeeId);
        _enrollments.Add(enrollment);
    }

    public void UnenrollEmployee(Guid employeeId)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.EmployeeId == employeeId);
        if (enrollment == null)
            throw new DomainException("Çalışan bu eğitime kayıtlı değil.");

        if (Status == TrainingStatus.InProgress || Status == TrainingStatus.Completed)
            throw new DomainException("Başlamış veya tamamlanmış eğitimden çıkış yapılamaz.");

        _enrollments.Remove(enrollment);
    }

    public void Start()
    {
        if (Status != TrainingStatus.RegistrationOpen)
            throw new DomainException("Eğitim kayıtlara açık değil.");

        if (_enrollments.Count == 0)
            throw new DomainException("Hiç kayıtlı katılımcı yok.");

        Status = TrainingStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TrainingStatus.InProgress)
            throw new DomainException("Sadece devam eden eğitimler tamamlanabilir.");

        Status = TrainingStatus.Completed;

        // Tüm kayıtları tamamlandı olarak işaretle
        foreach (var enrollment in _enrollments)
        {
            enrollment.Complete();
        }
    }

    public void Cancel()
    {
        if (Status == TrainingStatus.Completed)
            throw new DomainException("Tamamlanmış eğitim iptal edilemez.");

        Status = TrainingStatus.Cancelled;
    }

    public void Postpone(DateTime newStartDate, DateTime newEndDate)
    {
        if (Status == TrainingStatus.Completed || Status == TrainingStatus.Cancelled)
            throw new DomainException("Tamamlanmış veya iptal edilmiş eğitim ertelenemez.");

        if (newStartDate >= newEndDate)
            throw new DomainException("Yeni bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        StartDate = newStartDate;
        EndDate = newEndDate;
        Status = TrainingStatus.Postponed;
    }

    public bool IsEmployeeEnrolled(Guid employeeId)
    {
        return _enrollments.Any(e => e.EmployeeId == employeeId);
    }

    public int GetAvailableSeats()
    {
        return MaxParticipants - _enrollments.Count;
    }

    public bool IsFull() => _enrollments.Count >= MaxParticipants;

    public decimal GetCompletionRate()
    {
        if (_enrollments.Count == 0)
            return 0;

        var completedCount = _enrollments.Count(e => e.IsCompleted);
        return (decimal)completedCount / _enrollments.Count * 100;
    }
}

/// <summary>
/// Eğitim Kayıt Entity
/// </summary>
public class TrainingEnrollment : AuditableEntity
{
    public Guid TrainingId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletionDate { get; private set; }
    public decimal? Score { get; private set; }
    public string? Certificate { get; private set; }

    // Navigation Properties
    public Training Training { get; private set; } = null!;
    public Employee Employee { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private TrainingEnrollment() { }

    private TrainingEnrollment(Guid trainingId, Guid employeeId)
    {
        TrainingId = trainingId;
        EmployeeId = employeeId;
        EnrollmentDate = DateTime.UtcNow;
        IsCompleted = false;
    }

    public static TrainingEnrollment Create(Guid trainingId, Guid employeeId)
    {
        return new TrainingEnrollment(trainingId, employeeId);
    }

    public void Complete(decimal? score = null, string? certificate = null)
    {
        if (IsCompleted)
            throw new DomainException("Eğitim zaten tamamlanmış.");

        if (score.HasValue && (score < 0 || score > 100))
            throw new DomainException("Puan 0-100 arasında olmalıdır.");

        IsCompleted = true;
        CompletionDate = DateTime.UtcNow;
        Score = score;
        Certificate = certificate;
    }
}

/// <summary>
/// İzin Entity
/// </summary>
public class Leave : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public LeaveType LeaveType { get; private set; }
    public LeaveStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int TotalDays { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? AttachmentPath { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public DateTime? RejectedDate { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Employee Employee { get; private set; } = null!;
    public Employee? Approver { get; private set; }
    public Employee? Rejector { get; private set; }

    // Parameterless constructor for EF Core
    private Leave() { }

    private Leave(
        Guid employeeId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string reason,
        string? attachmentPath = null,
        string? notes = null)
    {
        EmployeeId = employeeId;
        LeaveType = leaveType;
        Status = LeaveStatus.Pending;
        StartDate = startDate;
        EndDate = endDate;
        TotalDays = CalculateTotalDays(startDate, endDate);
        Reason = reason;
        AttachmentPath = attachmentPath;
        Notes = notes;
    }

    public static Leave Create(
        Guid employeeId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string reason,
        string? attachmentPath = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("İzin sebebi belirtilmelidir.");

        if (startDate < DateTime.UtcNow.Date)
            throw new DomainException("İzin başlangıç tarihi geçmiş olamaz.");

        if (endDate < startDate)
            throw new DomainException("İzin bitiş tarihi başlangıç tarihinden önce olamaz.");

        var totalDays = CalculateTotalDays(startDate, endDate);

        // Maksimum izin süresi kontrolü
        if (totalDays > 90 && leaveType != LeaveType.Unpaid && leaveType != LeaveType.Sick)
            throw new DomainException("İzin süresi 90 günü aşamaz.");

        // Hastalık izni için rapor kontrolü
        if (leaveType == LeaveType.Sick && totalDays > 3 && string.IsNullOrWhiteSpace(attachmentPath))
            throw new DomainException("3 günden uzun hastalık izni için rapor belgesi gereklidir.");

        return new Leave(employeeId, leaveType, startDate, endDate, reason, attachmentPath, notes);
    }

    public void Approve(Guid approverId)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("Sadece beklemedeki izinler onaylanabilir.");

        Status = LeaveStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;
    }

    public void Reject(Guid rejectorId, string rejectionReason)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("Sadece beklemedeki izinler reddedilebilir.");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new DomainException("Red sebebi belirtilmelidir.");

        Status = LeaveStatus.Rejected;
        RejectedBy = rejectorId;
        RejectedDate = DateTime.UtcNow;
        RejectionReason = rejectionReason;
    }

    public void Cancel()
    {
        if (Status != LeaveStatus.Pending && Status != LeaveStatus.Approved)
            throw new DomainException("Sadece beklemedeki veya onaylı izinler iptal edilebilir.");

        if (StartDate <= DateTime.UtcNow)
            throw new DomainException("Başlamış izinler iptal edilemez.");

        Status = LeaveStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != LeaveStatus.Approved)
            throw new DomainException("Sadece onaylı izinler tamamlanabilir.");

        if (DateTime.UtcNow < EndDate)
            throw new DomainException("İzin henüz bitmedi.");

        Status = LeaveStatus.Completed;
    }

    public void AttachDocument(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("Dosya yolu boş olamaz.");

        AttachmentPath = filePath;
    }

    public void UpdateDates(DateTime newStartDate, DateTime newEndDate)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("Sadece beklemedeki izinlerin tarihleri güncellenebilir.");

        if (newStartDate < DateTime.UtcNow.Date)
            throw new DomainException("Yeni başlangıç tarihi geçmiş olamaz.");

        if (newEndDate < newStartDate)
            throw new DomainException("Yeni bitiş tarihi başlangıç tarihinden önce olamaz.");

        StartDate = newStartDate;
        EndDate = newEndDate;
        TotalDays = CalculateTotalDays(newStartDate, newEndDate);
    }

    public bool IsCurrentlyOnLeave()
    {
        if (Status != LeaveStatus.Approved)
            return false;

        var now = DateTime.UtcNow.Date;
        return now >= StartDate && now <= EndDate;
    }

    public bool IsUpcoming()
    {
        return Status == LeaveStatus.Approved && StartDate > DateTime.UtcNow.Date;
    }

    public bool IsPaid()
    {
        return LeaveType != LeaveType.Unpaid;
    }

    public int GetRemainingDays()
    {
        if (!IsCurrentlyOnLeave())
            return 0;

        var remaining = (EndDate - DateTime.UtcNow.Date).Days;
        return Math.Max(0, remaining);
    }

    private static int CalculateTotalDays(DateTime startDate, DateTime endDate)
    {
        // İzin günlerini hesapla (başlangıç ve bitiş günü dahil)
        var totalDays = (endDate - startDate).Days + 1;

        // Hafta sonlarını çıkar (isteğe bağlı - iş kuralına göre)
        var workingDays = 0;
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }

        return workingDays;
    }

    public int GetWorkingDays()
    {
        return TotalDays;
    }

    public bool OverlapsWith(Leave other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }
}