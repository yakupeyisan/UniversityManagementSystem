using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.HRAggregate;

/// <summary>
/// Eğitim Entity
/// </summary>
public class Training : AuditableEntity
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TrainingType Type { get; private set; }
    public TrainingStatus Status { get; private set; }
    public TrainingDuration Duration { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MaxParticipants { get; private set; }
    public string Instructor { get; private set; } = null!;
    public string? Location { get; private set; }
    public decimal? Cost { get; private set; }
    public string? Materials { get; private set; }
    public bool IsCertified { get; private set; }

    // Kayıtlı çalışanlar
    private readonly List<TrainingEnrollment> _enrollments = new();
    public IReadOnlyCollection<TrainingEnrollment> Enrollments => _enrollments.AsReadOnly();

    // Parameterless constructor for EF Core
    private Training() { }

    private Training(
        string title,
        string description,
        TrainingType type,
        TrainingDuration duration,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        string instructor,
        string? location = null,
        decimal? cost = null,
        bool isCertified = false)
    {
        Title = title;
        Description = description;
        Type = type;
        Status = TrainingStatus.Planned;
        Duration = duration;
        StartDate = startDate;
        EndDate = endDate;
        MaxParticipants = maxParticipants;
        Instructor = instructor;
        Location = location;
        Cost = cost;
        IsCertified = isCertified;
    }

    public static Training Create(
        string title,
        string description,
        TrainingType type,
        TrainingDuration duration,
        DateTime startDate,
        DateTime endDate,
        int maxParticipants,
        string instructor,
        string? location = null,
        decimal? cost = null,
        bool isCertified = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Eğitim başlığı boş olamaz.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Eğitim açıklaması boş olamaz.");

        if (string.IsNullOrWhiteSpace(instructor))
            throw new DomainException("Eğitmen belirtilmelidir.");

        if (startDate >= endDate)
            throw new DomainException("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        if (maxParticipants <= 0)
            throw new DomainException("Maksimum katılımcı sayısı pozitif olmalıdır.");

        return new Training(title, description, type, duration, startDate, endDate,
            maxParticipants, instructor, location, cost, isCertified);
    }

    public void OpenRegistration()
    {
        if (Status != TrainingStatus.Planned)
            throw new DomainException("Sadece planlanmış eğitimler için kayıt açılabilir.");

        Status = TrainingStatus.RegistrationOpen;
    }

    public void EnrollEmployee(Guid employeeId)
    {
        if (Status != TrainingStatus.RegistrationOpen)
            throw new DomainException("Kayıtlar açık değil.");

        if (_enrollments.Count >= MaxParticipants)
            throw new DomainException("Eğitim dolu.");

        if (_enrollments.Any(e => e.EmployeeId == employeeId))
            throw new DomainException("Çalışan zaten kayıtlı.");

        var enrollment = TrainingEnrollment.Create(Id, employeeId);
        _enrollments.Add(enrollment);
    }

    public void UnenrollEmployee(Guid employeeId)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.EmployeeId == employeeId);
        if (enrollment == null)
            throw new DomainException("Çalışan bu eğitime kayıtlı değil.");

        if (Status == TrainingStatus.InProgress || Status == TrainingStatus.Completed)
            throw new DomainException("Başlamış veya tamamlanmış eğitimden çıkış yapılamaz.");

        _enrollments.Remove(enrollment);
    }

    public void Start()
    {
        if (Status != TrainingStatus.RegistrationOpen)
            throw new DomainException("Eğitim kayıtlara açık değil.");

        if (_enrollments.Count == 0)
            throw new DomainException("Hiç kayıtlı katılımcı yok.");

        Status = TrainingStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != TrainingStatus.InProgress)
            throw new DomainException("Sadece devam eden eğitimler tamamlanabilir.");

        Status = TrainingStatus.Completed;

        // Tüm kayıtları tamamlandı olarak işaretle
        foreach (var enrollment in _enrollments)
        {
            enrollment.Complete();
        }
    }

    public void Cancel()
    {
        if (Status == TrainingStatus.Completed)
            throw new DomainException("Tamamlanmış eğitim iptal edilemez.");

        Status = TrainingStatus.Cancelled;
    }

    public void Postpone(DateTime newStartDate, DateTime newEndDate)
    {
        if (Status == TrainingStatus.Completed || Status == TrainingStatus.Cancelled)
            throw new DomainException("Tamamlanmış veya iptal edilmiş eğitim ertelenemez.");

        if (newStartDate >= newEndDate)
            throw new DomainException("Yeni bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        StartDate = newStartDate;
        EndDate = newEndDate;
        Status = TrainingStatus.Postponed;
    }

    public bool IsEmployeeEnrolled(Guid employeeId)
    {
        return _enrollments.Any(e => e.EmployeeId == employeeId);
    }

    public int GetAvailableSeats()
    {
        return MaxParticipants - _enrollments.Count;
    }

    public bool IsFull() => _enrollments.Count >= MaxParticipants;

    public decimal GetCompletionRate()
    {
        if (_enrollments.Count == 0)
            return 0;

        var completedCount = _enrollments.Count(e => e.IsCompleted);
        return (decimal)completedCount / _enrollments.Count * 100;
    }
}