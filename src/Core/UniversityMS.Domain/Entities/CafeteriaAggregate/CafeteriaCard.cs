using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

public class CafeteriaCard : AuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public string CardNumber { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = null!;
    public Money TotalSpent { get; private set; } = null!;
    public CafeteriaSubscriptionType? SubscriptionType { get; private set; }
    public DateTime? SubscriptionStartDate { get; private set; }
    public DateTime? SubscriptionEndDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastTransactionDate { get; private set; }

    // Collections
    private readonly List<CafeteriaTransaction> _transactions = new();
    public IReadOnlyCollection<CafeteriaTransaction> Transactions => _transactions.AsReadOnly();

    private CafeteriaCard() { }

    private CafeteriaCard(Guid userId, string cardNumber, decimal initialBalance)
    {
        UserId = userId;
        CardNumber = cardNumber;
        Balance = Money.Create(initialBalance, "TRY");
        TotalSpent = Money.Create(0, "TRY");
        IsActive = true;
    }

    public static CafeteriaCard Create(Guid userId, string cardNumber, decimal initialBalance = 0)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Kullanıcı ID boş olamaz.");
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new DomainException("Kart numarası boş olamaz.");
        if (initialBalance < 0)
            throw new DomainException("Başlangıç bakiyesi negatif olamaz.");

        return new CafeteriaCard(userId, cardNumber.Trim(), initialBalance);
    }

    public void LoadBalance(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Yükleme tutarı pozitif olmalı.");

        Balance = Money.Create(Balance.Amount + amount, "TRY");
        LastTransactionDate = DateTime.UtcNow;
    }

    public void Deduct(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Çekim tutarı pozitif olmalı.");

        bool hasActiveSubscription = IsSubscriptionActive();
        if (!hasActiveSubscription && Balance.Amount < amount)
            throw new DomainException("Yetersiz bakiye.");

        if (!hasActiveSubscription)
        {
            Balance = Money.Create(Balance.Amount - amount, "TRY");
        }

        TotalSpent = Money.Create(TotalSpent.Amount + amount, "TRY");
        LastTransactionDate = DateTime.UtcNow;
    }

    public void StartSubscription(CafeteriaSubscriptionType subscriptionType)
    {
        SubscriptionType = subscriptionType;
        SubscriptionStartDate = DateTime.UtcNow;
        SubscriptionEndDate = subscriptionType switch
        {
            CafeteriaSubscriptionType.Daily => DateTime.UtcNow.AddDays(1),
            CafeteriaSubscriptionType.Weekly => DateTime.UtcNow.AddDays(7),
            CafeteriaSubscriptionType.Monthly => DateTime.UtcNow.AddMonths(1),
            CafeteriaSubscriptionType.Semester => DateTime.UtcNow.AddMonths(6),
            _ => throw new DomainException("Geçersiz abonelik türü.")
        };
    }

    public bool IsSubscriptionActive() =>
        SubscriptionType.HasValue &&
        SubscriptionEndDate.HasValue &&
        DateTime.UtcNow <= SubscriptionEndDate;

    public void CancelSubscription()
    {
        SubscriptionType = null;
        SubscriptionStartDate = null;
        SubscriptionEndDate = null;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}