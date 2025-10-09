using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Application.Common.Interfaces;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Username { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permissions { get; }
    bool IsInRole(string role);
    bool HasPermission(string permission);
}
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body,
        byte[] attachment, string attachmentName);
}

public interface ISmsService
{
    Task SendSmsAsync(string phoneNumber, string message);
    Task SendBulkSmsAsync(IEnumerable<string> phoneNumbers, string message);
}

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<Student> Students { get; }
    DbSet<Staff> Staffs { get; }
    DbSet<Address> Addresses { get; }
    DbSet<EmergencyContact> EmergencyContacts { get; }

    public DbSet<Faculty> Faculties { get; }
    public DbSet<Department> Departments { get; }
    public DbSet<Course> Courses { get; }
    public DbSet<Prerequisite> Prerequisites { get; }
    public DbSet<Curriculum> Curriculums { get; }
    public DbSet<CurriculumCourse> CurriculumCourses { get; }
    public DbSet<Enrollment> Enrollments { get; }
    public DbSet<CourseRegistration> CourseRegistrations { get; }
    public DbSet<Grade> Grades { get; }
    public DbSet<Attendance> Attendances { get; }
    public DbSet<GradeObjection> GradeObjections { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}