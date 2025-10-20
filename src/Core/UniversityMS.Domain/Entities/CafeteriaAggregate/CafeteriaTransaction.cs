using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

public class CafeteriaTransaction : AuditableEntity
{
    public Guid CardId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public CafeteriaTransactionStatus Status { get; private set; }
    public string? ProductName { get; private set; }

    // Navigation
    public CafeteriaCard Card { get; private set; } = null!;

    private CafeteriaTransaction() { }

    private CafeteriaTransaction(Guid cardId, Guid userId, decimal amount,
        string description, string? productName)
    {
        CardId = cardId;
        UserId = userId;
        Amount = Money.Create(amount, "TRY");
        Description = description;
        ProductName = productName;
        Status = CafeteriaTransactionStatus.Success;
        TransactionDate = DateTime.UtcNow;
    }

    public static CafeteriaTransaction Create(Guid cardId, Guid userId, decimal amount,
        string description, string? productName = null)
    {
        if (cardId == Guid.Empty)
            throw new DomainException("Kart ID boş olamaz.");
        if (userId == Guid.Empty)
            throw new DomainException("Kullanıcı ID boş olamaz.");
        if (amount <= 0)
            throw new DomainException("İşlem tutarı pozitif olmalı.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        return new CafeteriaTransaction(cardId, userId, amount,
            description.Trim(), productName?.Trim());
    }

    public void MarkAsFailed() => Status = CafeteriaTransactionStatus.Failed;
    public void MarkAsRefunded() => Status = CafeteriaTransactionStatus.Refunded;
}