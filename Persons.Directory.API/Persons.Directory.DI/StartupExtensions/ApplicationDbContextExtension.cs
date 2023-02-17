using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Persistence.Db;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class ApplicationDbContextExtension
    {
        private const string assembly = "Persons.Directory.Persistence";

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options 
                => options.UseSqlServer(connectionString, b => b.MigrationsAssembly(assembly))
                          .UseLazyLoadingProxies());

            return services;
        }
    }
}
