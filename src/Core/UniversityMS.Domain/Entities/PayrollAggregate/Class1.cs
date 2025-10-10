using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PayrollAggregate;

/// <summary>
/// Bordro (Payroll) - Aggregate Root
/// Aylık/Dönemsel bordro hesaplama ve ödeme yönetimi
/// </summary>
public class Payroll : AuditableEntity, IAggregateRoot
{
    public string PayrollNumber { get; private set; } = null!;
    public Guid EmployeeId { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public PayrollPeriod Period { get; private set; } = null!;
    public PayrollStatus Status { get; private set; }

    // Maaş Bilgileri
    public Money BaseSalary { get; private set; } = null!;
    public Money TotalEarnings { get; private set; } = null!;
    public Money TotalDeductions { get; private set; } = null!;
    public Money NetSalary { get; private set; } = null!;

    // İş Günü Bilgileri
    public int WorkingDays { get; private set; }
    public int ActualWorkDays { get; private set; }
    public decimal OvertimeHours { get; private set; }
    public int LeaveDays { get; private set; }
    public int AbsentDays { get; private set; }

    // Ödeme Bilgileri
    public DateTime? PaymentDate { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? BankAccount { get; private set; }
    public string? PaymentReference { get; private set; }

    // Onay Bilgileri
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public Guid? PaidBy { get; private set; }
    public DateTime? PaidDate { get; private set; }

    public string? Notes { get; private set; }

    // Navigation Properties
    public Employee Employee { get; private set; } = null!;
    public Employee? Approver { get; private set; }
    public Employee? Payer { get; private set; }

    // Collections
    private readonly List<PayrollItem> _items = new();
    public IReadOnlyCollection<PayrollItem> Items => _items.AsReadOnly();

    private readonly List<PayrollDeduction> _deductions = new();
    public IReadOnlyCollection<PayrollDeduction> Deductions => _deductions.AsReadOnly();

    // Parameterless constructor for EF Core
    private Payroll() { }

    private Payroll(
        string payrollNumber,
        Guid employeeId,
        int year,
        int month,
        Money baseSalary,
        int workingDays,
        PaymentMethod paymentMethod,
        string? bankAccount = null)
    {
        PayrollNumber = payrollNumber;
        EmployeeId = employeeId;
        Year = year;
        Month = month;
        Period = PayrollPeriod.Create(year, month);
        Status = PayrollStatus.Draft;
        BaseSalary = baseSalary;
        WorkingDays = workingDays;
        ActualWorkDays = workingDays;
        PaymentMethod = paymentMethod;
        BankAccount = bankAccount;

        // Başlangıç değerleri
        TotalEarnings = baseSalary;
        TotalDeductions = Money.Zero();
        NetSalary = baseSalary;
    }

    public static Payroll Create(
        string payrollNumber,
        Guid employeeId,
        int year,
        int month,
        Money baseSalary,
        int workingDays,
        PaymentMethod paymentMethod,
        string? bankAccount = null)
    {
        if (string.IsNullOrWhiteSpace(payrollNumber))
            throw new DomainException("Bordro numarası boş olamaz.");

        if (year < 2000 || year > 2100)
            throw new DomainException("Geçerli bir yıl giriniz.");

        if (month < 1 || month > 12)
            throw new DomainException("Ay 1-12 arasında olmalıdır.");

        if (workingDays <= 0 || workingDays > 31)
            throw new DomainException("Çalışma günü sayısı geçersiz.");

        if (paymentMethod == PaymentMethod.BankTransfer && string.IsNullOrWhiteSpace(bankAccount))
            throw new DomainException("Banka havalesi için hesap numarası gereklidir.");

        return new Payroll(payrollNumber, employeeId, year, month, baseSalary, workingDays, paymentMethod, bankAccount);
    }

    #region Item Management

    public void AddItem(PayrollItem item)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrolara kalem eklenebilir.");

        if (item.PayrollId != Id)
            throw new DomainException("Bordro kalemi bu bordroya ait değil.");

        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrodan kalem silinebilir.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
        }
    }

    #endregion

    #region Deduction Management

    public void AddDeduction(PayrollDeduction deduction)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrolara kesinti eklenebilir.");

        if (deduction.PayrollId != Id)
            throw new DomainException("Kesinti bu bordroya ait değil.");

        _deductions.Add(deduction);
        RecalculateTotals();
    }

    public void RemoveDeduction(Guid deductionId)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrodan kesinti silinebilir.");

        var deduction = _deductions.FirstOrDefault(d => d.Id == deductionId);
        if (deduction != null)
        {
            _deductions.Remove(deduction);
            RecalculateTotals();
        }
    }

    #endregion

    #region Calculation Methods

    public void UpdateWorkingDays(int actualWorkDays, int leaveDays, int absentDays)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrolarda iş günü güncellenebilir.");

        if (actualWorkDays < 0 || leaveDays < 0 || absentDays < 0)
            throw new DomainException("Gün sayıları negatif olamaz.");

        ActualWorkDays = actualWorkDays;
        LeaveDays = leaveDays;
        AbsentDays = absentDays;

        RecalculateTotals();
    }

    public void UpdateOvertimeHours(decimal overtimeHours)
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrolarda fazla mesai güncellenebilir.");

        if (overtimeHours < 0)
            throw new DomainException("Fazla mesai negatif olamaz.");

        OvertimeHours = overtimeHours;
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        // Temel maaş hesaplama (eksik gün kesintisi)
        var dailySalary = BaseSalary.Amount / WorkingDays;
        var adjustedBaseSalary = Money.Create(dailySalary * ActualWorkDays, BaseSalary.Currency);

        // Ek ödemeler toplamı
        var totalItems = _items
            .Where(i => i.Type == PayrollItemType.Earning)
            .Sum(i => i.Amount.Amount);

        TotalEarnings = Money.Create(adjustedBaseSalary.Amount + totalItems, BaseSalary.Currency);

        // Kesintiler toplamı
        var totalDeductionAmount = _deductions.Sum(d => d.Amount.Amount);
        TotalDeductions = Money.Create(totalDeductionAmount, BaseSalary.Currency);

        // Net maaş
        NetSalary = Money.Create(TotalEarnings.Amount - TotalDeductions.Amount, BaseSalary.Currency);
    }

    #endregion

    #region Status Management

    public void Calculate()
    {
        if (Status != PayrollStatus.Draft)
            throw new DomainException("Sadece taslak bordrolar hesaplanabilir.");

        RecalculateTotals();
        Status = PayrollStatus.Calculated;

        AddDomainEvent(new PayrollCalculatedEvent(Id, EmployeeId, Period, NetSalary));
    }

    public void Approve(Guid approverId)
    {
        if (Status != PayrollStatus.Calculated)
            throw new DomainException("Sadece hesaplanmış bordrolar onaylanabilir.");

        Status = PayrollStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new PayrollApprovedEvent(Id, EmployeeId, approverId));
    }

    public void Reject(string reason)
    {
        if (Status != PayrollStatus.Calculated)
            throw new DomainException("Sadece hesaplanmış bordrolar reddedilebilir.");

        Status = PayrollStatus.Rejected;
        Notes = $"Red sebebi: {reason}";

        AddDomainEvent(new PayrollRejectedEvent(Id, EmployeeId, reason));
    }

    public void MarkAsPaid(Guid paidBy, string paymentReference)
    {
        if (Status != PayrollStatus.Approved)
            throw new DomainException("Sadece onaylı bordrolar ödenebilir.");

        if (string.IsNullOrWhiteSpace(paymentReference))
            throw new DomainException("Ödeme referansı gereklidir.");

        Status = PayrollStatus.Paid;
        PaidBy = paidBy;
        PaidDate = DateTime.UtcNow;
        PaymentDate = DateTime.UtcNow;
        PaymentReference = paymentReference;

        AddDomainEvent(new PayrollPaidEvent(Id, EmployeeId, NetSalary, PaymentDate.Value));
    }

    public void Cancel()
    {
        if (Status == PayrollStatus.Paid)
            throw new DomainException("Ödenmiş bordro iptal edilemez.");

        Status = PayrollStatus.Cancelled;

        AddDomainEvent(new PayrollCancelledEvent(Id, EmployeeId));
    }

    public void RevertToDraft()
    {
        if (Status == PayrollStatus.Paid)
            throw new DomainException("Ödenmiş bordro taslağa döndürülemez.");

        if (Status == PayrollStatus.Approved)
            throw new DomainException("Onaylı bordro taslağa döndürülemez.");

        Status = PayrollStatus.Draft;
        ApprovedBy = null;
        ApprovedDate = null;
    }

    #endregion

    #region Helper Methods

    public decimal GetTaxAmount()
    {
        return _deductions
            .Where(d => d.Type == DeductionType.Tax)
            .Sum(d => d.Amount.Amount);
    }

    public decimal GetSocialSecurityAmount()
    {
        return _deductions
            .Where(d => d.Type == DeductionType.SocialSecurity)
            .Sum(d => d.Amount.Amount);
    }

    public decimal GetOvertimePay()
    {
        return _items
            .Where(i => i.Type == PayrollItemType.Earning && i.Category == "Overtime")
            .Sum(i => i.Amount.Amount);
    }

    public bool IsPastDue()
    {
        // Bordro, ayın 5'inden sonra ödenmemişse gecikmiş sayılır
        var dueDate = new DateTime(Year, Month, 5).AddMonths(1);
        return Status != PayrollStatus.Paid && DateTime.UtcNow > dueDate;
    }

    #endregion
}

/// <summary>
/// Bordro Kalemi Entity
/// Maaşa eklenen veya çıkarılan kalemler (prim, ödül, fazla mesai vs.)
/// </summary>
public class PayrollItem : AuditableEntity
{
    public Guid PayrollId { get; private set; }
    public PayrollItemType Type { get; private set; }
    public string Category { get; private set; } = null!; // "Overtime", "Bonus", "Allowance" vs.
    public string Description { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public decimal? Quantity { get; private set; } // Miktar (ör: fazla mesai saati)
    public bool IsTaxable { get; private set; }

    // Navigation Property
    public Payroll Payroll { get; private set; } = null!;

    private PayrollItem() { }

    private PayrollItem(
        Guid payrollId,
        PayrollItemType type,
        string category,
        string description,
        Money amount,
        decimal? quantity = null,
        bool isTaxable = true)
    {
        PayrollId = payrollId;
        Type = type;
        Category = category;
        Description = description;
        Amount = amount;
        Quantity = quantity;
        IsTaxable = isTaxable;
    }

    public static PayrollItem Create(
        Guid payrollId,
        PayrollItemType type,
        string category,
        string description,
        Money amount,
        decimal? quantity = null,
        bool isTaxable = true)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("Kategori boş olamaz.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        if (quantity.HasValue && quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new PayrollItem(payrollId, type, category, description, amount, quantity, isTaxable);
    }

    public void UpdateAmount(Money newAmount)
    {
        Amount = newAmount;
    }
}

/// <summary>
/// Bordro Kesinti Entity
/// SGK, Gelir Vergisi, Damga Vergisi, Sendika Aidatı vs.
/// </summary>
public class PayrollDeduction : AuditableEntity
{
    public Guid PayrollId { get; private set; }
    public DeductionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public decimal? Rate { get; private set; } // Oran (%)
    public bool IsStatutory { get; private set; } // Yasal kesinti mi?
    public string? Reference { get; private set; } // İlgili yasal madde/referans

    // Navigation Property
    public Payroll Payroll { get; private set; } = null!;

    private PayrollDeduction() { }

    private PayrollDeduction(
        Guid payrollId,
        DeductionType type,
        string description,
        Money amount,
        decimal? rate = null,
        bool isStatutory = false,
        string? reference = null)
    {
        PayrollId = payrollId;
        Type = type;
        Description = description;
        Amount = amount;
        Rate = rate;
        IsStatutory = isStatutory;
        Reference = reference;
    }

    public static PayrollDeduction Create(
        Guid payrollId,
        DeductionType type,
        string description,
        Money amount,
        decimal? rate = null,
        bool isStatutory = false,
        string? reference = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        if (rate.HasValue && (rate <= 0 || rate > 100))
            throw new DomainException("Oran 0-100 arasında olmalıdır.");

        return new PayrollDeduction(payrollId, type, description, amount, rate, isStatutory, reference);
    }

    public void UpdateAmount(Money newAmount)
    {
        Amount = newAmount;
    }
}