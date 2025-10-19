using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.HRAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.HREvents;
using UniversityMS.Domain.Events.PayrollEvents;
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
    public string? BankName { get; private set; }
    public string? IBAN { get; private set; }

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
        // Türk mevzuatı kuralları:
        // ═════════════════════════════
        // WorkingDays = Ayda toplam iş günü (örn: 22 gün)
        // ActualWorkDays = Gerçek çalışılan gün (eksik gün kesintisiz)
        // LeaveDays = Ücretli izinler (raporlu, mazeret) - PARA KESILMIYOR
        // AbsentDays = Cezasız gün (izinsiz) - PARA KESILIR
        //
        // Formül: Net Ücret = (BaseSalary / WorkingDays * ActualWorkDays) + Ek Ödemeler - Kesintiler

        // Temel maaş hesaplama
        // Günlük maaş = Toplam maaş / Ayın iş günü
        var dailySalary = BaseSalary.Amount / WorkingDays;

        // Çalışılan günler = İş günü - İzinsiz gün
        // Raporlu izin/Mazeret izni dahil DEĞİL (zaten PAID)
        var paidWorkingDays = ActualWorkDays;

        // Prorata maaş (eksik gün kesintisi uygulanmış)
        var adjustedBaseSalary = Money.Create(
            dailySalary * paidWorkingDays,
            BaseSalary.Currency);


        // Ek ödemeler toplamı (fazla mesai, prim, tazminat, vb)
        var totalEarningsItems = _items
            .Where(i => i.Type == PayrollItemType.Earning)
            .Sum(i => i.Amount.Amount);


        // Toplam kazanç
        TotalEarnings = Money.Create(
            adjustedBaseSalary.Amount + totalEarningsItems,
            BaseSalary.Currency);

        // Kesintiler toplamı (gelir vergisi, SGK, sendika, vs)
        var totalDeductionAmount = _deductions
            .Sum(d => d.Amount.Amount);

        TotalDeductions = Money.Create(
            totalDeductionAmount,
            BaseSalary.Currency);

        // Net maaş (Brüt - Kesintiler)
        NetSalary = Money.Create(
            TotalEarnings.Amount - TotalDeductions.Amount,
            BaseSalary.Currency);

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

    /// <summary>
    /// Banka bilgisini ayarla (opsiyonel)
    /// </summary>
    public void SetBankInfo(string bankName, string? accountNumber = null)
    {
        BankName = bankName;
        BankAccount = accountNumber;
    }

    /// <summary>
    /// İşlem referansını ayarla
    /// </summary>
    public void SetTransactionReference(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new DomainException("İşlem referansı boş olamaz.");

        PaymentReference = reference;
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

    /// <summary>
    /// Payslip oluşturulduğunu işaretle
    /// </summary>
    public void GeneratePayslip(Guid payslipId)
    {
        if (Status != PayrollStatus.Approved)
            throw new DomainException("Sadece onaylı bordrodan payslip oluşturulabilir.");

        AddDomainEvent(new PayslipGeneratedEvent(Id, payslipId, EmployeeId));
    }

    #endregion

    public void SetActualWorkDays(int days)
    {
        if (days < 0 || days > 31)
            throw new DomainException("Çalışılan gün sayısı 0-31 arasında olmalıdır.");

        ActualWorkDays = days;
    }

    public void SetOvertimeHours(decimal hours)
    {
        if (hours < 0)
            throw new DomainException("Fazla mesai saati negatif olamaz.");

        OvertimeHours = hours;
    }
}