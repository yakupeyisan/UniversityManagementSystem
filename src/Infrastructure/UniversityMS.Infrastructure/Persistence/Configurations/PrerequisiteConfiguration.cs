using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

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