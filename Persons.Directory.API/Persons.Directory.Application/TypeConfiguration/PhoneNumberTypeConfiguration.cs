using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persons.Directory.Application.Domain;

namespace Persons.Directory.Application.TypeConfiguration
{
    public class PhoneNumberTypeConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("PhoneNumbers");

            builder.Property(x => x.Number).HasMaxLength(50);

            builder.HasOne(x => x.Person)
                .WithMany(x => x.PhoneNumbers)
                .HasForeignKey(x => x.PersonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
