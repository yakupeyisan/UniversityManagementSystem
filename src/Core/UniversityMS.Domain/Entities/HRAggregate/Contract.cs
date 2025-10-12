using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.HRAggregate;

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