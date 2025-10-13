using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.FacilityAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
{
    public void Configure(EntityTypeBuilder<Classroom> builder)
    {
        builder.ToTable("Classrooms");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Building)
            .HasMaxLength(100);

        builder.Property(c => c.Floor)
            .IsRequired();

        builder.Property(c => c.Capacity)
            .IsRequired();

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.HasProjector)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.HasSmartBoard)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.HasComputers)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.HasAirConditioning)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.ComputerCount);

        // Soft Delete
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt);
        builder.Property(c => c.DeletedBy).HasMaxLength(100);

        // Audit
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(100);
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.UpdatedBy).HasMaxLength(100);

        // Indexes
        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => c.IsActive)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => new { c.Building, c.Floor })
            .HasFilter("[IsDeleted] = 0");

        // Ignore domain events
        builder.Ignore(c => c.DomainEvents);
    }
}