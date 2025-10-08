namespace UniversityMS.Application.Features.Attendances.DTOs;

public record AttendanceDto(
    Guid StudentId,
    Guid CourseRegistrationId,
    bool IsPresent
);