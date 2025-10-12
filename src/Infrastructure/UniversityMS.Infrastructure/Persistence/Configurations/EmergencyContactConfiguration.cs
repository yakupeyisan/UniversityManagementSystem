using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class EmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("EmergencyContacts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Relationship).IsRequired().HasMaxLength(50);

        builder.OwnsOne(e => e.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .IsRequired()
                .HasMaxLength(11);
        });

        builder.OwnsOne(e => e.AlternativePhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("AlternativePhone")
                .HasMaxLength(11);
        });

        builder.Ignore(e => e.DomainEvents);
    }
}