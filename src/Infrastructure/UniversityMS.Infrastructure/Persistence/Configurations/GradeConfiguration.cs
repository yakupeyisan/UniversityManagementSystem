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
        builder.Property(g => g.UpdatedAt);
        builder.Property(g => g.UpdatedBy).HasMaxLength(100);


        builder.HasOne(g => g.CourseRegistration)
            .WithMany(cr => cr.Grades)
            .HasForeignKey(g => g.CourseRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Student)
            .WithMany(s => s.Grades)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Course)
            .WithMany()
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Instructor)
            .WithMany(s => s.GradesEntered)
            .HasForeignKey(g => g.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(g => g.GradeObjections)
            .WithOne(go => go.Grade)
            .HasForeignKey(go => go.GradeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(g => g.DomainEvents);
    }
}