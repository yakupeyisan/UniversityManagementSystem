using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.ScheduleAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class CourseSessionConfiguration : IEntityTypeConfiguration<CourseSession>
{
    public void Configure(EntityTypeBuilder<CourseSession> builder)
    {
        builder.ToTable("CourseSessions");

        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.ScheduleId).IsRequired();
        builder.Property(cs => cs.CourseId).IsRequired();
        builder.Property(cs => cs.InstructorId);
        builder.Property(cs => cs.ClassroomId).IsRequired();


        builder.Property(cs => cs.DayOfWeek)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cs => cs.SessionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cs => cs.Notes)
            .HasMaxLength(500);

        // TimeSlot Value Object
        builder.OwnsOne(cs => cs.TimeSlot, timeSlot =>
        {
            timeSlot.Property(t => t.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();

            timeSlot.Property(t => t.EndTime)
                .HasColumnName("EndTime")
                .IsRequired();
        });

        // Soft Delete
        builder.Property(cs => cs.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(cs => cs.DeletedAt);
        builder.Property(cs => cs.DeletedBy).HasMaxLength(100);

        // Audit
        builder.Property(cs => cs.CreatedAt).IsRequired();
        builder.Property(cs => cs.CreatedBy).HasMaxLength(100);
        builder.Property(cs => cs.UpdatedAt);
        builder.Property(cs => cs.UpdatedBy).HasMaxLength(100);

        // Indexes
        builder.HasIndex(cs => new { cs.ScheduleId, cs.DayOfWeek })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(cs => cs.CourseId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(cs => cs.InstructorId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(cs => cs.ClassroomId)
            .HasFilter("[IsDeleted] = 0");

        // Relationships
        builder.HasOne(cs => cs.Course)
            .WithMany()
            .HasForeignKey(cs => cs.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Classroom)
            .WithMany()
            .HasForeignKey(cs => cs.ClassroomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cs => cs.Instructor)
            .WithMany(s => s.CourseSessions)
            .HasForeignKey(cs => cs.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cs => cs.Schedule)
            .WithMany(s => s.CourseSessions)
            .HasForeignKey(cs => cs.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(cs => cs.DomainEvents);
    }
}