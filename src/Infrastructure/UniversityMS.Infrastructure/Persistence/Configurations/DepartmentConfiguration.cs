using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(d => d.Code).IsUnique();

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.IsActive).IsRequired().HasDefaultValue(true);

        // Soft Delete
        builder.Property(d => d.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(d => d.DeletedAt);
        builder.Property(d => d.DeletedBy).HasMaxLength(100);

        // Audit fields
        builder.Property(d => d.CreatedAt).IsRequired();
        builder.Property(d => d.CreatedBy).HasMaxLength(100);
        builder.Property(d => d.UpdatedAt);
        builder.Property(d => d.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(d => d.Faculty)
            .WithMany(f => f.Departments)
            .HasForeignKey(d => d.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.HeadOfDepartment)
            .WithMany(s => s.ManagedDepartments)
            .HasForeignKey(d => d.HeadOfDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(d => d.Courses)
            .WithOne(c => c.Department)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(d => d.DomainEvents);
    }
}