using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.LibraryEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.LibraryAggregate;

/// <summary>
/// Ödünç Alma (Loan) Entity
/// </summary>
public class Loan : AuditableEntity
{
    public Guid MaterialId { get; private set; }
    public Guid BorrowerId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
    public int RenewalCount { get; private set; }
    public int MaxRenewals { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CheckedOutBy { get; private set; }
    public Guid? CheckedInBy { get; private set; }

    // Navigation Properties
    public Material Material { get; private set; } = null!;
    public Person Borrower { get; private set; } = null!;

    private Loan() { }

    private Loan(
        Guid materialId,
        Guid borrowerId,
        DateTime loanDate,
        DateTime dueDate,
        int maxRenewals = 2,
        Guid? checkedOutBy = null)
    {
        MaterialId = materialId;
        BorrowerId = borrowerId;
        LoanDate = loanDate;
        DueDate = dueDate;
        Status = LoanStatus.Active;
        RenewalCount = 0;
        MaxRenewals = maxRenewals;
        CheckedOutBy = checkedOutBy;
    }

    public static Loan Create(
        Guid materialId,
        Guid borrowerId,
        DateTime loanDate,
        int loanPeriodDays = 14,
        int maxRenewals = 2,
        Guid? checkedOutBy = null)
    {
        if (loanPeriodDays <= 0)
            throw new DomainException("Ödünç alma süresi pozitif olmalıdır.");

        var dueDate = loanDate.AddDays(loanPeriodDays);
        var loan = new Loan(materialId, borrowerId, loanDate, dueDate, maxRenewals, checkedOutBy);

        loan.AddDomainEvent(new MaterialLoanedEvent(materialId, borrowerId, loanDate, dueDate));

        return loan;
    }

    public void Return(Guid? checkedInBy = null, string? notes = null)
    {
        if (Status == LoanStatus.Returned)
            throw new DomainException("Materyal zaten iade edilmiş.");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;
        CheckedInBy = checkedInBy;
        if (!string.IsNullOrWhiteSpace(notes))
            Notes = notes;

        AddDomainEvent(new MaterialReturnedEvent(MaterialId, BorrowerId, ReturnDate.Value));
    }

    public void Renew(int additionalDays = 14)
    {
        if (Status != LoanStatus.Active)
            throw new DomainException("Sadece aktif ödünçler yenilenebilir.");

        if (RenewalCount >= MaxRenewals)
            throw new DomainException($"Maksimum {MaxRenewals} kez yenilenebilir.");

        if (IsOverdue())
            throw new DomainException("Gecikmiş ödünçler yenilenemez.");

        DueDate = DueDate.AddDays(additionalDays);
        RenewalCount++;
        Status = LoanStatus.Renewed;

        AddDomainEvent(new LoanRenewedEvent(Id, MaterialId, DueDate));
    }

    public void MarkAsOverdue()
    {
        if (Status == LoanStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = LoanStatus.Overdue;
            var overdueDays = (DateTime.UtcNow - DueDate).Days;
            AddDomainEvent(new MaterialOverdueEvent(Id, MaterialId, BorrowerId, overdueDays));
        }
    }

    public void MarkAsLost()
    {
        Status = LoanStatus.Lost;
    }

    public void MarkAsDamaged(string notes)
    {
        Status = LoanStatus.Damaged;
        Notes = notes;
    }

    public bool IsOverdue()
    {
        return Status != LoanStatus.Returned && DateTime.UtcNow > DueDate;
    }

    public int GetOverdueDays()
    {
        if (!IsOverdue()) return 0;
        return (DateTime.UtcNow - DueDate).Days;
    }

    public bool CanBeRenewed()
    {
        return Status == LoanStatus.Active &&
               RenewalCount < MaxRenewals &&
               !IsOverdue();
    }
}