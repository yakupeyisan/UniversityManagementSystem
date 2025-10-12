using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.AcademicAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class CurriculumCourseConfiguration : IEntityTypeConfiguration<CurriculumCourse>
{
    public void Configure(EntityTypeBuilder<CurriculumCourse> builder)
    {
        builder.ToTable("CurriculumCourses");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Semester).IsRequired();
        builder.Property(cc => cc.CourseType).IsRequired();
        builder.Property(cc => cc.IsElective).IsRequired();

        // Aynı müfredatta aynı ders tekrar eklenemez
        builder.HasIndex(cc => new { cc.CurriculumId, cc.CourseId }).IsUnique();

        // Audit fields
        builder.Property(cc => cc.CreatedAt).IsRequired();
        builder.Property(cc => cc.CreatedBy).HasMaxLength(100);
        builder.Property(cc => cc.UpdatedAt);
        builder.Property(cc => cc.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(cc => cc.Curriculum)
            .WithMany(c => c.CurriculumCourses)
            .HasForeignKey(cc => cc.CurriculumId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cc => cc.Course)
            .WithMany()
            .HasForeignKey(cc => cc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(cc => cc.DomainEvents);
    }
}