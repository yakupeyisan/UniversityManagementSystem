using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

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