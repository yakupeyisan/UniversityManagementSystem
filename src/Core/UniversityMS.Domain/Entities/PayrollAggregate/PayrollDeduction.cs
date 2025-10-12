using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PayrollAggregate;

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