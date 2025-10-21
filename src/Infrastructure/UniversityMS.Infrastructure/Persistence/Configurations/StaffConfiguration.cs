using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("Staffs");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.EmployeeNumber).IsUnique();

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.NationalId)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(s => s.NationalId);

        // Value Objects
        builder.OwnsOne(s => s.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);
        });

        builder.OwnsOne(s => s.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(11);
        });


        // Staff specific fields
        builder.Property(s => s.JobTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.AcademicTitle);
        builder.Property(s => s.HireDate).IsRequired();
        builder.Property(s => s.TerminationDate);
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.WeeklyWorkload);
        builder.Property(s => s.AdviseeCount);

        // Soft Delete
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(s => s.Address)
            .WithMany()
            .HasForeignKey("AddressId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.EmergencyContact)
            .WithMany()
            .HasForeignKey("EmergencyContactId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.ManagedDepartments)
            .WithOne(d => d.HeadOfDepartment)
            .HasForeignKey(d => d.HeadOfDepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.CourseSessions)
            .WithOne(cs => cs.Instructor)
            .HasForeignKey(cs => cs.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.GradesEntered)
            .WithOne(g => g.Instructor)
            .HasForeignKey(g => g.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(s => s.DomainEvents);
    }
}