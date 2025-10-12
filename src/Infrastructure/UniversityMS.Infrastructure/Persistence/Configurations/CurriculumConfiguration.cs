using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

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
        builder.Property(c => c.StartYear).IsRequired();
        builder.Property(c => c.EndYear);
        builder.Property(c => c.TotalECTS).IsRequired();
        builder.Property(c => c.TotalRequiredECTS).IsRequired();
        builder.Property(c => c.TotalRequiredNationalCredit).IsRequired();
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);

        // Soft Delete
        builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(c => c.DeletedAt);
        builder.Property(c => c.DeletedBy).HasMaxLength(100);

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