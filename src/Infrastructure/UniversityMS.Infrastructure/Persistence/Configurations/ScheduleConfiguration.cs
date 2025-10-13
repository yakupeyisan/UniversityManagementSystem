using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.ToTable("Schedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AcademicYear)
            .IsRequired()
            .HasMaxLength(9); // 2024-2025

        builder.Property(s => s.Semester)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate)
            .IsRequired();

        builder.Property(s => s.PublishedDate);
        builder.Property(s => s.PublishedBy);
        builder.Property(s => s.DepartmentId);

        // Soft Delete
        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.DeletedAt);
        builder.Property(s => s.DeletedBy).HasMaxLength(100);

        // Audit
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).HasMaxLength(100);
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.UpdatedBy).HasMaxLength(100);

        // Indexes
        builder.HasIndex(s => new { s.AcademicYear, s.Semester, s.DepartmentId })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(s => s.Status);

        // Relationships
        builder.HasMany(s => s.CourseSessions)
            .WithOne(cs => cs.Schedule)
            .HasForeignKey(cs => cs.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(s => s.DomainEvents);
    }
}