using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

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
        builder.Property(e => e.DeletedAt);
        builder.Property(e => e.DeletedBy).HasMaxLength(100);

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