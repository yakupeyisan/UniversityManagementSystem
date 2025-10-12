using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

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