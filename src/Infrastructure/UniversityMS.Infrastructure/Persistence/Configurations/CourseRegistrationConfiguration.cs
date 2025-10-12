using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.EnrollmentAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class CourseRegistrationConfiguration : IEntityTypeConfiguration<CourseRegistration>
{
    public void Configure(EntityTypeBuilder<CourseRegistration> builder)
    {
        builder.ToTable("CourseRegistrations");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.InstructorId);
        builder.Property(cr => cr.ECTS).IsRequired();
        builder.Property(cr => cr.NationalCredit).IsRequired();
        builder.Property(cr => cr.Status).IsRequired();
        builder.Property(cr => cr.RegistrationDate).IsRequired();
        builder.Property(cr => cr.DropDate);

        // Bir kayıtta aynı ders tekrar eklenemez
        builder.HasIndex(cr => new { cr.EnrollmentId, cr.CourseId }).IsUnique();

        // Soft Delete
        builder.Property(cr => cr.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(cr => cr.DeletedAt);
        builder.Property(cr => cr.DeletedBy).HasMaxLength(100);

        // Audit fields
        builder.Property(cr => cr.CreatedAt).IsRequired();
        builder.Property(cr => cr.CreatedBy).HasMaxLength(100);
        builder.Property(cr => cr.UpdatedAt);
        builder.Property(cr => cr.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(cr => cr.Enrollment)
            .WithMany(e => e.CourseRegistrations)
            .HasForeignKey(cr => cr.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.Course)
            .WithMany()
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cr => cr.Grades)
            .WithOne(g => g.CourseRegistration)
            .HasForeignKey(g => g.CourseRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cr => cr.Attendances)
            .WithOne(a => a.CourseRegistration)
            .HasForeignKey(a => a.CourseRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(cr => cr.DomainEvents);
    }
}