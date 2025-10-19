namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class AttendanceRecordDto
{
    public Guid Id { get; set; }
    public int WeekNumber { get; set; }
    public bool IsPresent { get; set; }
    public DateTime RecordedAt { get; set; }
    public string? QRCode { get; set; }
}