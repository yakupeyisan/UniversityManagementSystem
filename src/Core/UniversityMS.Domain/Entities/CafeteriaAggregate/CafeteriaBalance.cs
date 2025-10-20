using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

/// <summary>
/// Yemekhane Bakiyesi
/// </summary>
public class CafeteriaBalance : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Money CurrentBalance { get; private set; } = null!;
    public Money TotalLoaded { get; private set; } = null!;
    public Money TotalSpent { get; private set; } = null!;
    public DateTime? LastLoadDate { get; private set; }
    public DateTime? LastSpendDate { get; private set; }
    public bool IsActive { get; private set; }

    public Person User { get; private set; } = null!;

    private CafeteriaBalance() { }

    private CafeteriaBalance(Guid userId)
    {
        UserId = userId;
        CurrentBalance = Money.Zero();
        TotalLoaded = Money.Zero();
        TotalSpent = Money.Zero();
        IsActive = true;
    }

    public static CafeteriaBalance Create(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Kullanıcı kimliği geçersizdir.");
        return new CafeteriaBalance(userId);
    }

    public void LoadBalance(Money amount)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Yükleme tutarı pozitif olmalıdır.");
        CurrentBalance = Money.Create(CurrentBalance.Amount + amount.Amount, CurrentBalance.Currency);
        TotalLoaded = Money.Create(TotalLoaded.Amount + amount.Amount, TotalLoaded.Currency);
        LastLoadDate = DateTime.UtcNow;
    }

    public void SpendBalance(Money amount)
    {
        if (amount.Amount <= 0 || CurrentBalance.Amount < amount.Amount)
            throw new DomainException("Yetersiz bakiye.");
        CurrentBalance = Money.Create(CurrentBalance.Amount - amount.Amount, CurrentBalance.Currency);
        TotalSpent = Money.Create(TotalSpent.Amount + amount.Amount, TotalSpent.Currency);
        LastSpendDate = DateTime.UtcNow;
    }
}