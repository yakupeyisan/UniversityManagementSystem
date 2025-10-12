using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.LibraryEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.LibraryAggregate;


/// <summary>
/// Kütüphane Materyali (Material) - Aggregate Root
/// Kitap, dergi, tez vb. tüm kütüphane materyallerini temsil eder
/// </summary>
public class Material : AuditableEntity, IAggregateRoot
{
    public string ISBN { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Subtitle { get; private set; }
    public string Author { get; private set; } = null!;
    public string? Publisher { get; private set; }
    public int? PublicationYear { get; private set; }
    public string? Edition { get; private set; }
    public MaterialType Type { get; private set; }
    public MaterialStatus Status { get; private set; }
    public string? Language { get; private set; }
    public int PageCount { get; private set; }
    public string? Subject { get; private set; }
    public string? Category { get; private set; }
    public string? ShelfLocation { get; private set; }
    public string? Barcode { get; private set; }
    public Money? PurchasePrice { get; private set; }
    public DateTime? AcquisitionDate { get; private set; }
    public string? Description { get; private set; }
    public string? Keywords { get; private set; }
    public bool IsReferenceOnly { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }

    // Collections
    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly();

    private readonly List<Reservation> _reservations = new();
    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

    private Material() { }

    private Material(
        string isbn,
        string title,
        string author,
        MaterialType type,
        int totalCopies,
        string? subtitle = null,
        string? publisher = null,
        int? publicationYear = null,
        bool isReferenceOnly = false)
    {
        ISBN = isbn;
        Title = title;
        Subtitle = subtitle;
        Author = author;
        Publisher = publisher;
        PublicationYear = publicationYear;
        Type = type;
        Status = MaterialStatus.InProcessing;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
        IsReferenceOnly = isReferenceOnly;
    }

    public static Material Create(
        string isbn,
        string title,
        string author,
        MaterialType type,
        int totalCopies = 1,
        string? subtitle = null,
        string? publisher = null,
        int? publicationYear = null,
        bool isReferenceOnly = false)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new DomainException("ISBN boş olamaz.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Başlık boş olamaz.");

        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("Yazar boş olamaz.");

        if (totalCopies <= 0)
            throw new DomainException("Toplam kopya sayısı pozitif olmalıdır.");

        var material = new Material(isbn, title, author, type, totalCopies, subtitle, publisher, publicationYear, isReferenceOnly);
        material.AddDomainEvent(new MaterialAddedEvent(material.Id, isbn, title));

        return material;
    }

    #region Material Management

    public void MakeAvailable()
    {
        if (Status == MaterialStatus.InProcessing)
        {
            Status = MaterialStatus.Available;
        }
    }

    public void UpdateLocation(string shelfLocation)
    {
        if (string.IsNullOrWhiteSpace(shelfLocation))
            throw new DomainException("Raf konumu boş olamaz.");

        ShelfLocation = shelfLocation;
    }

    public void UpdateDetails(
        string? subtitle = null,
        string? publisher = null,
        int? publicationYear = null,
        string? edition = null,
        string? language = null,
        int? pageCount = null,
        string? description = null)
    {
        Subtitle = subtitle;
        Publisher = publisher;
        PublicationYear = publicationYear;
        Edition = edition;
        Language = language;
        if (pageCount.HasValue) PageCount = pageCount.Value;
        Description = description;
    }

    public void MarkAsLost()
    {
        Status = MaterialStatus.Lost;
        AvailableCopies = Math.Max(0, AvailableCopies - 1);
    }

    public void MarkAsDamaged()
    {
        Status = MaterialStatus.Damaged;
        AvailableCopies = Math.Max(0, AvailableCopies - 1);
    }

    public void RepairCompleted()
    {
        if (Status == MaterialStatus.Damaged || Status == MaterialStatus.UnderMaintenance)
        {
            Status = MaterialStatus.Available;
            AvailableCopies++;
        }
    }

    #endregion

    #region Loan Management

    public bool CanBeLoan()
    {
        return !IsReferenceOnly &&
               AvailableCopies > 0 &&
               Status == MaterialStatus.Available;
    }

    public void LoanOut()
    {
        if (!CanBeLoan())
            throw new DomainException("Bu materyal ödünç verilemez.");

        AvailableCopies--;
        if (AvailableCopies == 0)
            Status = MaterialStatus.OnLoan;
    }

    public void ReturnLoan()
    {
        AvailableCopies++;
        if (AvailableCopies > 0 && Status == MaterialStatus.OnLoan)
            Status = MaterialStatus.Available;
    }

    public void AddLoan(Loan loan)
    {
        if (loan.MaterialId != Id)
            throw new DomainException("Ödünç kayıt bu materyale ait değil.");

        _loans.Add(loan);
    }

    #endregion

    #region Reservation Management

    public bool CanBeReserved()
    {
        return Status == MaterialStatus.Available ||
               Status == MaterialStatus.OnLoan;
    }

    public void AddReservation(Reservation reservation)
    {
        if (reservation.MaterialId != Id)
            throw new DomainException("Rezervasyon bu materyale ait değil.");

        if (IsReferenceOnly)
            throw new DomainException("Kaynak kitaplar rezerve edilemez.");

        _reservations.Add(reservation);

        if (Status == MaterialStatus.Available)
            Status = MaterialStatus.Reserved;
    }

    public void RemoveReservation(Guid reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);
        if (reservation != null)
        {
            _reservations.Remove(reservation);

            // Eğer başka rezervasyon yoksa ve materyal müsaitse
            if (!_reservations.Any(r => r.Status == ReservationStatus.Active) &&
                Status == MaterialStatus.Reserved)
            {
                Status = MaterialStatus.Available;
            }
        }
    }

    public Reservation? GetNextReservation()
    {
        return _reservations
            .Where(r => r.Status == ReservationStatus.Active)
            .OrderBy(r => r.ReservationDate)
            .FirstOrDefault();
    }

    #endregion

    #region Query Methods

    public int GetActiveLoansCount()
    {
        return _loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue);
    }

    public int GetActiveReservationsCount()
    {
        return _reservations.Count(r => r.Status == ReservationStatus.Active);
    }

    public bool IsAvailable()
    {
        return Status == MaterialStatus.Available && AvailableCopies > 0;
    }

    #endregion
}