using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.LibraryEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.LibraryAggregate;

/// <summary>
/// Kütüphane Cezası (Fine) Entity
/// </summary>
public class Fine : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid? LoanId { get; private set; }
    public Guid? MaterialId { get; private set; }
    public FineType Type { get; private set; }
    public FineStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public Money PaidAmount { get; private set; } = null!;
    public Money RemainingAmount { get; private set; } = null!;
    public DateTime IssuedDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? PaidDate { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? PaymentReference { get; private set; }
    public Guid? WaivedBy { get; private set; }
    public string? WaiverReason { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Person User { get; private set; } = null!;
    public Loan? Loan { get; private set; }
    public Material? Material { get; private set; }

    private Fine() { }

    private Fine(
        Guid userId,
        FineType type,
        Money amount,
        string reason,
        Guid? loanId = null,
        Guid? materialId = null,
        int dueDays = 30)
    {
        UserId = userId;
        LoanId = loanId;
        MaterialId = materialId;
        Type = type;
        Status = FineStatus.Pending;
        Amount = amount;
        PaidAmount = Money.Zero();
        RemainingAmount = amount;
        IssuedDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(dueDays);
        Reason = reason;
    }

    public static Fine Create(
        Guid userId,
        FineType type,
        Money amount,
        string reason,
        Guid? loanId = null,
        Guid? materialId = null,
        int dueDays = 30)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Ceza sebebi belirtilmelidir.");

        if (amount.Amount <= 0)
            throw new DomainException("Ceza tutarı pozitif olmalıdır.");

        var fine = new Fine(userId, type, amount, reason, loanId, materialId, dueDays);
        fine.AddDomainEvent(new FineIssuedEvent(fine.Id, userId, amount, reason));

        return fine;
    }

    #region Payment Operations

    public void Pay(Money paymentAmount, string? paymentReference = null)
    {
        if (Status == FineStatus.Paid)
            throw new DomainException("Ceza zaten ödenmiş.");

        if (Status == FineStatus.Waived)
            throw new DomainException("Affedilmiş ceza ödenemez.");

        if (Status == FineStatus.Cancelled)
            throw new DomainException("İptal edilmiş ceza ödenemez.");

        if (paymentAmount.Amount <= 0)
            throw new DomainException("Ödeme tutarı pozitif olmalıdır.");

        if (paymentAmount.Amount > RemainingAmount.Amount)
            throw new DomainException("Ödeme tutarı kalan borçtan fazla olamaz.");

        PaidAmount = Money.Create(PaidAmount.Amount + paymentAmount.Amount, Amount.Currency);
        RemainingAmount = Money.Create(RemainingAmount.Amount - paymentAmount.Amount, Amount.Currency);
        PaymentReference = paymentReference;

        if (RemainingAmount.Amount <= 0)
        {
            Status = FineStatus.Paid;
            PaidDate = DateTime.UtcNow;
            AddDomainEvent(new FinePaidEvent(Id, UserId, Amount, PaidDate.Value));
        }
        else
        {
            Status = FineStatus.PartiallyPaid;
        }
    }

    public void Waive(Guid waivedBy, string reason)
    {
        if (Status == FineStatus.Paid)
            throw new DomainException("Ödenmiş ceza affedilemez.");

        if (Status == FineStatus.Cancelled)
            throw new DomainException("İptal edilmiş ceza affedilemez.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Af sebebi belirtilmelidir.");

        Status = FineStatus.Waived;
        WaivedBy = waivedBy;
        WaiverReason = reason;
        RemainingAmount = Money.Zero();
    }

    public void Cancel(string reason)
    {
        if (Status == FineStatus.Paid)
            throw new DomainException("Ödenmiş ceza iptal edilemez.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("İptal sebebi belirtilmelidir.");

        Status = FineStatus.Cancelled;
        Notes = $"İptal sebebi: {reason}";
        RemainingAmount = Money.Zero();
    }

    #endregion

    #region Helper Methods

    public bool IsOverdue()
    {
        return Status == FineStatus.Pending &&
               DueDate.HasValue &&
               DateTime.UtcNow > DueDate.Value;
    }

    public bool IsPaid()
    {
        return Status == FineStatus.Paid;
    }

    public bool IsWaived()
    {
        return Status == FineStatus.Waived;
    }

    public decimal GetPaymentPercentage()
    {
        if (Amount.Amount == 0) return 100;
        return (PaidAmount.Amount / Amount.Amount) * 100;
    }

    public int GetDaysUntilDue()
    {
        if (!DueDate.HasValue) return 0;
        if (IsOverdue()) return 0;
        return (DueDate.Value - DateTime.UtcNow).Days;
    }

    public int GetOverdueDays()
    {
        if (!DueDate.HasValue || !IsOverdue()) return 0;
        return (DateTime.UtcNow - DueDate.Value).Days;
    }

    #endregion

    #region Static Factory Methods

    public static Fine CreateLateFee(
        Guid userId,
        Guid loanId,
        Guid materialId,
        int overdueDays,
        decimal dailyFee = 1.0m)
    {
        var amount = Money.Create(overdueDays * dailyFee, "TRY");
        var reason = $"{overdueDays} gün gecikme ücreti";

        return Create(userId, FineType.LateFee, amount, reason, loanId, materialId);
    }

    public static Fine CreateDamageFee(
        Guid userId,
        Guid loanId,
        Guid materialId,
        Money repairCost,
        string damageDescription)
    {
        var reason = $"Hasar bedeli: {damageDescription}";
        return Create(userId, FineType.DamageFee, repairCost, reason, loanId, materialId);
    }

    public static Fine CreateLostItemFee(
        Guid userId,
        Guid loanId,
        Guid materialId,
        Money replacementCost)
    {
        var reason = "Kayıp materyal değiştirme bedeli";
        return Create(userId, FineType.LostItemFee, replacementCost, reason, loanId, materialId);
    }

    #endregion
}