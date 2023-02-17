using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persons.Directory.Application.Domain;

namespace Persons.Directory.Application.TypeConfiguration;

public class CityTypeConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Cities");

        builder.Property(x => x.NameKa).HasMaxLength(30);
        builder.Property(x => x.NameEn).HasMaxLength(30);
    }
}