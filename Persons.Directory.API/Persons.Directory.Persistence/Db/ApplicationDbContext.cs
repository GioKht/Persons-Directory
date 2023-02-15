using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.TypeConfiguration;

namespace Persons.Directory.Persistence.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonTypeConfiguration());
        }
    }
}
