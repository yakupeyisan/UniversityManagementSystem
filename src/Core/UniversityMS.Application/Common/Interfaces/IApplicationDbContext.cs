using Microsoft.EntityFrameworkCore;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Identity
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }

    // Person Aggregate
    DbSet<Student> Students { get; }
    DbSet<Staff> Staffs { get; }
    DbSet<Address> Addresses { get; }
    DbSet<EmergencyContact> EmergencyContacts { get; }

    // Academic Aggregate
    DbSet<Faculty> Faculties { get; }
    DbSet<Department> Departments { get; }
    DbSet<Course> Courses { get; }
    DbSet<Prerequisite> Prerequisites { get; }
    DbSet<Curriculum> Curriculums { get; }
    DbSet<CurriculumCourse> CurriculumCourses { get; }

    // Enrollment Aggregate
    DbSet<Enrollment> Enrollments { get; }
    DbSet<CourseRegistration> CourseRegistrations { get; }
    DbSet<Grade> Grades { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<GradeObjection> GradeObjections { get; }

    // Schedule Aggregate
    DbSet<Schedule> Schedules { get; }
    DbSet<CourseSession> CourseSessions { get; }

    // Facility Aggregate
    DbSet<Classroom> Classrooms { get; }
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}