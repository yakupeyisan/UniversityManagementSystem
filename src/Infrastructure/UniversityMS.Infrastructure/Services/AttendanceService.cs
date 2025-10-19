using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IRepository<Attendance> _attendanceRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;

    public AttendanceService(
        IRepository<Attendance> attendanceRepository,
        IRepository<CourseRegistration> courseRegistrationRepository)
    {
        _attendanceRepository = attendanceRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
    }

    public async Task<decimal> GetStudentAttendancePercentageAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var courseRegistrations = await _courseRegistrationRepository.FindAsync(
            cr => cr.Enrollment.StudentId == studentId && cr.CourseId == courseId,
            cancellationToken);

        if (!courseRegistrations.Any())
            return 0;

        var courseRegistration = courseRegistrations.First();
        var attendances = await _attendanceRepository.FindAsync(
            a => a.CourseRegistrationId == courseRegistration.Id,
            cancellationToken);

        if (!attendances.Any())
            return 0;

        var presentCount = attendances.Count(a => a.IsPresent);
        return (presentCount * 100m) / attendances.Count;
    }

    public async Task<int> GetCourseAbsenceCountAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var courseRegistrations = await _courseRegistrationRepository.FindAsync(
            cr => cr.CourseId == courseId,
            cancellationToken);

        if (!courseRegistrations.Any())
            return 0;

        var crIds = courseRegistrations.Select(cr => cr.Id).ToList();
        var absences = 0;

        foreach (var crId in crIds)
        {
            var attendances = await _attendanceRepository.FindAsync(
                a => a.CourseRegistrationId == crId && !a.IsPresent,
                cancellationToken);
            absences += attendances.Count;
        }

        return absences;
    }

    public async Task<bool> IsStudentPresentTodayAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var courseRegistrations = await _courseRegistrationRepository.FindAsync(
            cr => cr.Enrollment.StudentId == studentId && cr.CourseId == courseId,
            cancellationToken);

        if (!courseRegistrations.Any())
            return false;

        var courseRegistration = courseRegistrations.First();
        var todayAttendance = await _attendanceRepository.FindAsync(
            a => a.CourseRegistrationId == courseRegistration.Id
                 && a.CreatedAt.Date == DateTime.UtcNow.Date,
            cancellationToken);

        return todayAttendance.Any(a => a.IsPresent);
    }
}