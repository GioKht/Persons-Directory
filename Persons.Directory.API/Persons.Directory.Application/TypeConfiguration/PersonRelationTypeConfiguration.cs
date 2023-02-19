using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persons.Directory.Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persons.Directory.Application.TypeConfiguration
{
    public class PersonRelationTypeConfiguration : IEntityTypeConfiguration<PersonRelation>
    {
        public void Configure(EntityTypeBuilder<PersonRelation> builder)
        {
            builder.HasKey(pr => new { pr.PersonId, pr.RelatedPersonId });

            //builder.HasOne(pr => pr.Person)
            //    .WithMany(p => p.RelatedPersons)
            //    .HasForeignKey(pr => pr.PersonId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(pr => pr.RelatedPerson)
            //    .WithMany()
            //    .HasForeignKey(pr => pr.RelatedPersonId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.Person)
                   .WithMany(p => p.RelatedPersons)
                   .HasForeignKey(pr => pr.PersonId);

            builder.HasOne(pr => pr.RelatedPerson)
                   .WithMany(p => p.RelatedToPersons)
                   .HasForeignKey(pr => pr.RelatedPersonId);
        }
    }
}
