using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.PayrollAggregate;

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