using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class EnrollmentValidationService : IEnrollmentValidationService
{
    private readonly IRepository<Enrollment> _enrollmentRepository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<Prerequisite> _prerequisiteRepository;
    private readonly IRepository<Grade> _gradeRepository;
    private readonly IRepository<CourseRegistration> _courseRegistrationRepository;
    private readonly IRepository<Schedule> _scheduleRepository;

    public EnrollmentValidationService(
        IRepository<Enrollment> enrollmentRepository,
        IRepository<Course> courseRepository,
        IRepository<Prerequisite> prerequisiteRepository,
        IRepository<Grade> gradeRepository,
        IRepository<CourseRegistration> courseRegistrationRepository, 
        IRepository<Schedule> scheduleRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _prerequisiteRepository = prerequisiteRepository;
        _gradeRepository = gradeRepository;
        _courseRegistrationRepository = courseRegistrationRepository;
        _scheduleRepository = scheduleRepository;
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
            foreach (var prerequisite in prerequisites)
            {
                if (!registeredCourseIds.Contains(prerequisite.PrerequisiteCourseId))
                    return false;

                // Ön koşul dersinden geçme notu alınmış mı?
                var grade = (await _gradeRepository.FindAsync(
                    g => g.CourseRegistration.Enrollment.StudentId == studentId &&
                         g.CourseRegistration.CourseId == prerequisite.PrerequisiteCourseId,
                    cancellationToken)).FirstOrDefault();

                // DÜZELTME: Grade.NumericScore property kullan
                if (grade == null || grade.NumericScore < 50)
                    return false;
            }
        }

        return true;
    }

    public async Task<bool> IsEnrollmentPeriodActiveAsync(string academicYear, int semester, CancellationToken cancellationToken)
    {
        var enrollments = await _enrollmentRepository.FindAsync(
            e => e.AcademicYear == academicYear && e.Semester == semester,
            cancellationToken);

        if (!enrollments.Any())
            return false;

        var schedule = (await _scheduleRepository.FindAsync(
            s => s.AcademicYear == academicYear && s.Semester == semester,
            cancellationToken)).FirstOrDefault();

        if (schedule == null)
            return false;

        var now = DateTime.UtcNow;
        return now >= schedule.StartDate && now <= schedule.EndDate;
    }

    public async Task<int> GetStudentCourseLoadAsync(Guid studentId, string academicYear, int semester, CancellationToken cancellationToken)
    {
        var registrations = await _courseRegistrationRepository.FindAsync(
            cr => cr.Enrollment.StudentId == studentId &&
                  cr.Enrollment.AcademicYear == academicYear &&
                  cr.Enrollment.Semester == semester,
            cancellationToken);

        // DÜZELTME: Course.ECTS property kullan
        return registrations.Sum(r => r.Course.ECTS);
    }
}