namespace UniversityMS.Application.Common.Interfaces;

public interface IAttendanceService
{
    Task<decimal> GetStudentAttendancePercentageAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task<int> GetCourseAbsenceCountAsync(Guid courseId, CancellationToken cancellationToken);
    Task<bool> IsStudentPresentTodayAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
}