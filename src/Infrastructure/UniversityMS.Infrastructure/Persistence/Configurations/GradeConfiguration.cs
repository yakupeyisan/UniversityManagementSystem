using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("Grades");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GradeType).IsRequired();
        builder.Property(g => g.NumericScore)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(g => g.LetterGrade)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(g => g.GradePoint)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(g => g.Weight)
            .IsRequired()
            .HasColumnType("decimal(3,2)");

        builder.Property(g => g.GradeDate).IsRequired();
        builder.Property(g => g.Notes).HasMaxLength(500);

        builder.HasIndex(g => new { g.CourseRegistrationId, g.GradeType }).IsUnique();

        // Audit
        builder.Property(g => g.CreatedAt).IsRequired();
        builder.Property(g => g.CreatedBy).HasMaxLength(100);

        builder.Ignore(g => g.DomainEvents);
    }
}