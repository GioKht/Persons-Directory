using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persons.Directory.Application.Domain;

namespace Persons.Directory.Application.TypeConfiguration;

public class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Persons");

        builder.Property(x => x.FirstName).HasMaxLength(50);
        builder.Property(x => x.LastName).HasMaxLength(50);
        builder.Property(x=> x.PersonalId).HasMaxLength(11);
    }
}
