using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityMS.Domain.Entities.PersonAggregate;

namespace UniversityMS.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street).IsRequired().HasMaxLength(200);
        builder.Property(a => a.BuildingNo).HasMaxLength(20);
        builder.Property(a => a.ApartmentNo).HasMaxLength(20);
        builder.Property(a => a.District).IsRequired().HasMaxLength(100);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PostalCode).IsRequired().HasMaxLength(10);
        builder.Property(a => a.Country).IsRequired().HasMaxLength(100);

        builder.Ignore(a => a.DomainEvents);
    }
}