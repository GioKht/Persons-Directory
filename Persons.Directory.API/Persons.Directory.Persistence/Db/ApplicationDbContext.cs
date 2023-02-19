using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.TypeConfiguration;
using System.Reflection;

namespace Persons.Directory.Persistence.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonTypeConfiguration())
                        .ApplyConfiguration(new PersonRelationTypeConfiguration())
                        .ApplyConfiguration(new PhoneNumberTypeConfiguration())
                        .ApplyConfiguration(new CityTypeConfiguration());
        }
    }
}
