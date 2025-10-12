using Microsoft.EntityFrameworkCore;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Student> Students { get; }
    DbSet<Staff> Staffs { get; }
    DbSet<Address> Addresses { get; }
    DbSet<EmergencyContact> EmergencyContacts { get; }
    DbSet<Faculty> Faculties { get; }
    DbSet<Department> Departments { get; }
    DbSet<Course> Courses { get; }
    DbSet<Prerequisite> Prerequisites { get; }
    DbSet<Curriculum> Curriculums { get; }
    DbSet<CurriculumCourse> CurriculumCourses { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<CourseRegistration> CourseRegistrations { get; }
    DbSet<Grade> Grades { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<GradeObjection> GradeObjections { get; }

    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}