using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class GradeObjectionConfiguration : IEntityTypeConfiguration<GradeObjection>
{
    public void Configure(EntityTypeBuilder<GradeObjection> builder)
    {
        builder.ToTable("GradeObjections");

        builder.HasKey(go => go.Id);

        builder.Property(go => go.Reason)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(go => go.Status).IsRequired();
        builder.Property(go => go.ObjectionDate).IsRequired();

        builder.Property(go => go.ReviewNotes)
            .HasMaxLength(1000);

        builder.Property(go => go.OldScore)
            .HasColumnType("decimal(5,2)");

        builder.Property(go => go.NewScore)
            .HasColumnType("decimal(5,2)");

        // Bir not için aynı anda sadece bir aktif itiraz olabilir
        builder.HasIndex(go => new { go.GradeId, go.Status });

        // Audit fields
        builder.Property(go => go.CreatedAt).IsRequired();
        builder.Property(go => go.CreatedBy).HasMaxLength(100);
        builder.Property(go => go.UpdatedAt);
        builder.Property(go => go.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(go => go.Grade)
            .WithMany()
            .HasForeignKey(go => go.GradeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(go => go.DomainEvents);
    }
}