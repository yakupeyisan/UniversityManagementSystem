namespace UniversityMS.Application.Common.Interfaces;

public interface IEnrollmentValidationService
{
    Task<bool> CanStudentEnrollAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task<bool> HasPrerequisitesAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);
    Task<bool> IsEnrollmentPeriodActiveAsync(string academicYear, int semester, CancellationToken cancellationToken);
    Task<int> GetStudentCourseLoadAsync(Guid studentId, string academicYear, int semester, CancellationToken cancellationToken);
}