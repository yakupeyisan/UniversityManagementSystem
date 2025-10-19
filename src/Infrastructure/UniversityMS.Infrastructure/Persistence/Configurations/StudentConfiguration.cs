using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(s => s.StudentNumber).IsUnique();

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

        builder.Property(s => s.BirthDate).IsRequired();
        builder.Property(s => s.Gender).IsRequired();

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

        builder.OwnsOne(s => s.Balance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Student specific fields
        builder.Property(s => s.DepartmentId).IsRequired();
        builder.Property(s => s.EducationLevel).IsRequired();
        builder.Property(s => s.CurrentSemester).IsRequired();
        builder.Property(s => s.Status).IsRequired();
        builder.Property(s => s.CGPA).HasColumnType("decimal(3,2)");
        builder.Property(s => s.SGPA).HasColumnType("decimal(3,2)");
        builder.Property(s => s.QRCode).HasMaxLength(200);
        builder.Property(s => s.CardNumber).HasMaxLength(16);
        builder.Property(s => s.ProfilePhotoUrl).HasMaxLength(500);

        // Soft Delete
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        // Audit fields
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.CreatedBy).HasMaxLength(100);
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.UpdatedBy).HasMaxLength(100);

        // Relationships
        builder.HasOne(s => s.Address)
            .WithMany()
            .HasForeignKey("AddressId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.EmergencyContact)
            .WithMany()
            .HasForeignKey("EmergencyContactId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Grades)
            .WithOne(g => g.Student)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(s => s.DomainEvents);
    }
}