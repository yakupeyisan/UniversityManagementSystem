using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.HRAggregate;

/// <summary>
/// İzin Entity
/// Çalışanların izin taleplerini yönetir
/// </summary>
public class Leave : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public LeaveType LeaveType { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int TotalDays { get; private set; }
    public LeaveStatus Status { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? RejectionReason { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public DateTime? RejectedDate { get; private set; }
    public string? DocumentPath { get; private set; } // Rapor, sertifika vs.
    public DateTime? CancelledDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Employee Employee { get; private set; } = null!;
    public Employee? Approver { get; private set; }
    public Employee? Rejector { get; private set; }

    // Parameterless constructor for EF Core
    private Leave() { }

    private Leave(
        Guid employeeId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string reason,
        string? documentPath = null)
    {
        EmployeeId = employeeId;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
        TotalDays = CalculateTotalDays(startDate, endDate);
        Status = LeaveStatus.Pending;
        Reason = reason;
        DocumentPath = documentPath;
    }

    public static Leave Create(
        Guid employeeId,
        LeaveType leaveType,
        DateTime startDate,
        DateTime endDate,
        string reason,
        string? documentPath = null)
    {
        if (startDate.Date < DateTime.UtcNow.Date)
            throw new DomainException("İzin başlangıç tarihi geçmiş olamaz.");

        if (endDate < startDate)
            throw new DomainException("İzin bitiş tarihi başlangıç tarihinden önce olamaz.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("İzin sebebi belirtilmelidir.");

        // Hastalık izni için rapor zorunlu
        if (leaveType == LeaveType.Sick && string.IsNullOrWhiteSpace(documentPath))
            throw new DomainException("Hastalık izni için rapor yüklenmesi zorunludur.");

        return new Leave(employeeId, leaveType, startDate, endDate, reason, documentPath);
    }

    public void Approve(Guid approverId)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("Sadece bekleyen izinler onaylanabilir.");

        Status = LeaveStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;
    }

    public void Reject(Guid rejectorId, string reason)
    {
        if (Status != LeaveStatus.Pending)
            throw new DomainException("Sadece bekleyen izinler reddedilebilir.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Red sebebi belirtilmelidir.");

        Status = LeaveStatus.Rejected;
        RejectedBy = rejectorId;
        RejectedDate = DateTime.UtcNow;
        RejectionReason = reason;
    }

    public void Cancel()
    {
        if (Status == LeaveStatus.Cancelled)
            throw new DomainException("İzin zaten iptal edilmiş.");

        if (Status == LeaveStatus.Completed)
            throw new DomainException("Tamamlanmış izin iptal edilemez.");

        if (StartDate.Date <= DateTime.UtcNow.Date)
            throw new DomainException("Başlamış veya geçmiş izin iptal edilemez.");

        Status = LeaveStatus.Cancelled;
        CancelledDate = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        if (Status != LeaveStatus.Approved)
            throw new DomainException("Sadece onaylı izinler tamamlanabilir.");

        if (DateTime.UtcNow.Date <= EndDate.Date)
            throw new DomainException("İzin henüz bitmemiş.");

        Status = LeaveStatus.Completed;
    }

    public void AttachDocument(string documentPath)
    {
        if (string.IsNullOrWhiteSpace(documentPath))
            throw new DomainException("Belge yolu boş olamaz.");

        DocumentPath = documentPath;
    }

    public bool IsCurrentlyOnLeave()
    {
        var today = DateTime.UtcNow.Date;
        return Status == LeaveStatus.Approved &&
               StartDate.Date <= today &&
               EndDate.Date >= today;
    }

    public bool IsUpcoming()
    {
        return Status == LeaveStatus.Approved &&
               StartDate.Date > DateTime.UtcNow.Date;
    }

    public int GetRemainingDays()
    {
        if (Status != LeaveStatus.Approved)
            return 0;

        var today = DateTime.UtcNow.Date;
        if (today > EndDate.Date)
            return 0;

        if (today < StartDate.Date)
            return TotalDays;

        return (int)(EndDate.Date - today).TotalDays + 1;
    }

    private static int CalculateTotalDays(DateTime startDate, DateTime endDate)
    {
        // Hafta sonları hariç iş günü hesaplama
        int totalDays = 0;
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            // Cumartesi ve Pazar hariç
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                totalDays++;
        }
        return totalDays;
    }
}