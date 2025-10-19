using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class EnrollmentValidationService : IEnrollmentValidationService
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Prerequisite> _prerequisiteRepository;
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;

    public EnrollmentValidationService(
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository,
        IRepository<Prerequisite> prerequisiteRepository,
        IRepository<Grade> gradeRepository,
        IRepository<CourseRegistration> courseRegistrationRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _prerequisiteRepository = prerequisiteRepository;
        _gradeRepository = gradeRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
    }

    public async Task<bool> CanStudentEnrollAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        // Öğrenci aktif mi?
        var enrollments = await _enrollmentRepository.FindAsync(
            e => e.StudentId == studentId,
            cancellationToken);

        if (!enrollments.Any())
            return false;

        // Ders zaten kayıtlı mı?
        var existingRegistrations = await _courseRegistrationRepository.FindAsync(
            cr => cr.Enrollment.StudentId == studentId && cr.CourseId == courseId,
            cancellationToken);

        if (existingRegistrations.Any())
            return false;

        // Ön koşulları sağlıyor mu?
        return await HasPrerequisitesAsync(studentId, courseId, cancellationToken);
    }

    public async Task<bool> HasPrerequisitesAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        var prerequisites = await _prerequisiteRepository.FindAsync(
            p => p.CourseId == courseId,
            cancellationToken);

        if (!prerequisites.Any())
            return true;

        var studentEnrollments = await _enrollmentRepository.FindAsync(
            e => e.StudentId == studentId,
            cancellationToken);

        foreach (var enrollment in studentEnrollments)
        {
            var registrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.EnrollmentId == enrollment.Id,
                cancellationToken);

            var registeredCourseIds = registrations.Select(r => r.CourseId).ToList();

            // Her ön koşul dersi tamamlanmış mı?
            foreach (var prereq in prerequisites)
            {
                if (!registeredCourseIds.Contains(prereq.PrerequisiteCourseId))
                    return false;

                // Dersten geçmiş mi (en az 50)?
                var grades = await _gradeRepository.FindAsync(
                    g => g.CourseRegistration.EnrollmentId == enrollment.Id
                         && g.CourseRegistration.CourseId == prereq.PrerequisiteCourseId
                         && g.NumericGrade >= 50,
                    cancellationToken);

                if (!grades.Any())
                    return false;
            }
        }

        return true;
    }

    public async Task<bool> IsEnrollmentPeriodActiveAsync(string academicYear, int semester, CancellationToken cancellationToken)
    {
        // Kayıt dönemi kontrolü (örnek: Eylül 1 - 30)
        var now = DateTime.UtcNow;
        var currentMonth = now.Month;

        if (semester == 1)
        {
            // Güz yarıyılı: Eylül-Ekim
            return currentMonth is 9 or 10;
        }
        else
        {
            // Bahar yarıyılı: Şubat-Mart
            return currentMonth is 2 or 3;
        }
    }

    public async Task<int> GetStudentCourseLoadAsync(
        Guid studentId,
        string academicYear,
        int semester,
        CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.FindAsync(
            e => e.StudentId == studentId && e.AcademicYear == academicYear && e.Semester == semester,
            cancellationToken);

        if (!enrollments.Any())
            return 0;

        var courseRegistrations = new List<CourseRegistration>();
        foreach (var enrollment in enrollments)
        {
            var registrations = await _courseRegistrationRepository.FindAsync(
                cr => cr.EnrollmentId == enrollment.Id,
                cancellationToken);
            courseRegistrations.AddRange(registrations);
        }

        return courseRegistrations.Sum(cr => cr.Course.Credits);
    }
}