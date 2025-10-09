using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.EnrollmentAggregate;
using UniversityMS.Domain.Entities.IdentityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Username).IsUnique();

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.HasIndex(u => u.Email.Value).IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.EmailConfirmed).IsRequired();
        builder.Property(u => u.RefreshToken).HasMaxLength(500);

        // Soft Delete
        builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.CreatedBy).HasMaxLength(100);
        builder.Property(u => u.UpdatedAt);
        builder.Property(u => u.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore navigation properties for domain events
        builder.Ignore(u => u.DomainEvents);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.IsSystemRole).IsRequired();

        // Audit fields
        builder.Property(r => r.CreatedAt).IsRequired();
        builder.Property(r => r.CreatedBy).HasMaxLength(100);

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions",
                j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"));

        builder.Ignore(r => r.DomainEvents);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Resource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description).HasMaxLength(500);

        // Audit fields
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(100);

        builder.Ignore(p => p.DomainEvents);
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.StudentNumber).IsUnique();

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.NationalId)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(s => s.NationalId);

        builder.Property(s => s.BirthDate).IsRequired();
        builder.Property(s => s.Gender).IsRequired();

        // Value Objects
        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.OwnsOne(s => s.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.OwnsOne(s => s.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Student specific fields
        builder.Property(s => s.DepartmentId).IsRequired();
        builder.Property(s => s.EducationLevel).IsRequired();
        builder.Property(s => s.CurrentSemester).IsRequired();
        builder.Property(s => s.Status).IsRequired();
        builder.Property(s => s.CGPA).HasColumnType("decimal(3,2)");
        builder.Property(s => s.SGPA).HasColumnType("decimal(3,2)");
        builder.Property(s => s.QRCode).HasMaxLength(200);
        builder.Property(s => s.CardNumber).HasMaxLength(16);
        builder.Property(s => s.ProfilePhotoUrl).HasMaxLength(500);

        // Soft Delete
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).HasMaxLength(100);
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(s => s.Address)
            .WithMany()
            .HasForeignKey("AddressId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.EmergencyContact)
            .WithMany()
            .HasForeignKey("EmergencyContactId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(s => s.DomainEvents);
    }
}

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staffs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.EmployeeNumber).IsUnique();

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.NationalId)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(s => s.NationalId);

        // Value Objects
        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.OwnsOne(s => s.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.OwnsOne(s => s.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Staff specific fields
        builder.Property(s => s.JobTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.AcademicTitle);
        builder.Property(s => s.HireDate).IsRequired();
        builder.Property(s => s.TerminationDate);
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.QRCode).HasMaxLength(200);
        builder.Property(s => s.WeeklyWorkload);
        builder.Property(s => s.AdviseeCount);

        // Soft Delete
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(s => s.Address)
            .WithMany()
            .HasForeignKey("AddressId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.EmergencyContact)
            .WithMany()
            .HasForeignKey("EmergencyContactId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(s => s.DomainEvents);
    }
}
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street).IsRequired().HasMaxLength(200);
        builder.Property(a => a.BuildingNo).HasMaxLength(20);
        builder.Property(a => a.ApartmentNo).HasMaxLength(20);
        builder.Property(a => a.District).IsRequired().HasMaxLength(100);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(10);
        builder.Property(a => a.Country).IsRequired().HasMaxLength(100);

        builder.Ignore(a => a.DomainEvents);
    }
}
public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Relationship).IsRequired().HasMaxLength(50);

        builder.OwnsOne(e => e.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.OwnsOne(e => e.AlternativePhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("AlternativePhone")
                .HasMaxLength(11);
        });

        builder.Ignore(e => e.DomainEvents);
    }
}

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("Grades");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GradeType).IsRequired();
        builder.Property(g => g.NumericScore)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(g => g.LetterGrade)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(g => g.GradePoint)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(g => g.Weight)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(g => g.GradeDate).IsRequired();
        builder.Property(g => g.Notes).HasMaxLength(500);

        builder.HasIndex(g => new { g.CourseRegistrationId, g.GradeType }).IsUnique();

        // Audit
        builder.Property(g => g.CreatedAt).IsRequired();
        builder.Property(g => g.CreatedBy).HasMaxLength(100);

        builder.Ignore(g => g.DomainEvents);
    }
}

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AttendanceDate).IsRequired();
        builder.Property(a => a.WeekNumber).IsRequired();
        builder.Property(a => a.IsPresent).IsRequired();
        builder.Property(a => a.Method).IsRequired();
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasIndex(a => new { a.CourseRegistrationId, a.AttendanceDate }).IsUnique();

        // Audit
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.CreatedBy).HasMaxLength(100);

        builder.Ignore(a => a.DomainEvents);
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.CourseType).IsRequired();
        builder.Property(c => c.TheoreticalHours).IsRequired();
        builder.Property(c => c.PracticalHours).IsRequired();
        builder.Property(c => c.ECTS).IsRequired();
        builder.Property(c => c.NationalCredit).IsRequired();
        builder.Property(c => c.EducationLevel).IsRequired();
        builder.Property(c => c.Semester);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

        // Soft Delete
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(c => c.Department)
            .WithMany(d => d.Courses)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Prerequisites)
            .WithOne(p => p.Course)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.DomainEvents);
    }
}

public class PrerequisiteConfiguration : IEntityTypeConfiguration<Prerequisite>
{
    public void Configure(EntityTypeBuilder<Prerequisite> builder)
    {
        builder.ToTable("Prerequisites");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CourseId).IsRequired();
        builder.Property(p => p.PrerequisiteCourseId).IsRequired();

        // Aynı ders için aynı önkoşul tekrar eklenemez
        builder.HasIndex(p => new { p.CourseId, p.PrerequisiteCourseId }).IsUnique();

        // Audit fields
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(p => p.Course)
            .WithMany(c => c.Prerequisites)
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.PrerequisiteCourse)
            .WithMany()
            .HasForeignKey(p => p.PrerequisiteCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(p => p.DomainEvents);
    }
}

public class CurriculumConfiguration : IEntityTypeConfiguration<Curriculum>
{
    public void Configure(EntityTypeBuilder<Curriculum> builder)
    {
        builder.ToTable("Curriculums");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.AcademicYear)
            .IsRequired()
            .HasMaxLength(10); // Örn: "2024-2025"

        builder.Property(c => c.EducationLevel).IsRequired();
        builder.Property(c => c.TotalECTS).IsRequired();
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

        // Soft Delete
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CurriculumCourses)
            .WithOne(cc => cc.Curriculum)
            .HasForeignKey(cc => cc.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.DomainEvents);
    }
}

public class CurriculumCourseConfiguration : IEntityTypeConfiguration<CurriculumCourse>
{
    public void Configure(EntityTypeBuilder<CurriculumCourse> builder)
    {
        builder.ToTable("CurriculumCourses");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Semester).IsRequired();
        builder.Property(cc => cc.CourseType).IsRequired();
        builder.Property(cc => cc.IsElective).IsRequired();

        // Aynı müfredatta aynı ders tekrar eklenemez
        builder.HasIndex(cc => new { cc.CurriculumId, cc.CourseId }).IsUnique();

        // Audit fields
        builder.Property(cc => cc.CreatedAt).IsRequired();
        builder.Property(cc => cc.CreatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(cc => cc.Curriculum)
            .WithMany(c => c.CurriculumCourses)
            .HasForeignKey(cc => cc.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cc => cc.Course)
            .WithMany()
            .HasForeignKey(cc => cc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(cc => cc.DomainEvents);
    }
}

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AcademicYear)
            .IsRequired()
            .HasMaxLength(10); // Örn: "2024-2025"

        builder.Property(e => e.Semester)
            .IsRequired();

        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.EnrollmentDate).IsRequired();
        builder.Property(e => e.ApprovalDate);
        builder.Property(e => e.ApprovedBy);
        builder.Property(e => e.TotalECTS).IsRequired().HasDefaultValue(0);
        builder.Property(e => e.TotalNationalCredit).IsRequired().HasDefaultValue(0);

        // Bir öğrencinin aynı dönem için sadece bir kaydı olabilir
        builder.HasIndex(e => new { e.StudentId, e.AcademicYear, e.Semester }).IsUnique();

        // Soft Delete
        builder.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.CourseRegistrations)
            .WithOne(cr => cr.Enrollment)
            .HasForeignKey(cr => cr.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.DomainEvents);
    }
}

public class CourseRegistrationConfiguration : IEntityTypeConfiguration<CourseRegistration>
{
    public void Configure(EntityTypeBuilder<CourseRegistration> builder)
    {
        builder.ToTable("CourseRegistrations");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.InstructorId);
        builder.Property(cr => cr.ECTS).IsRequired();
        builder.Property(cr => cr.NationalCredit).IsRequired();
        builder.Property(cr => cr.Status).IsRequired();
        builder.Property(cr => cr.RegistrationDate).IsRequired();
        builder.Property(cr => cr.DropDate);

        // Bir kayıtta aynı ders tekrar eklenemez
        builder.HasIndex(cr => new { cr.EnrollmentId, cr.CourseId }).IsUnique();

        // Soft Delete
        builder.Property(cr => cr.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(cr => cr.CreatedAt).IsRequired();
        builder.Property(cr => cr.CreatedBy).HasMaxLength(100);
        builder.Property(cr => cr.UpdatedAt);
        builder.Property(cr => cr.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(cr => cr.Enrollment)
            .WithMany(e => e.CourseRegistrations)
            .HasForeignKey(cr => cr.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cr => cr.Grades)
            .WithOne(g => g.CourseRegistration)
            .HasForeignKey(g => g.CourseRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cr => cr.Attendances)
            .WithOne(a => a.CourseRegistration)
            .HasForeignKey(a => a.CourseRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(cr => cr.DomainEvents);
    }
}