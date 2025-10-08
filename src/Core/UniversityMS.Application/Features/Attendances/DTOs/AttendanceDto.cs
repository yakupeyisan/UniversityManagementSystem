namespace UniversityMS.Application.Features.Attendances.DTOs;

public record AttendanceDto(
    Guid StudentId,
    bool IsPresent
);