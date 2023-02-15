using Microsoft.EntityFrameworkCore;

namespace Persons.Directory.Persistence.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
