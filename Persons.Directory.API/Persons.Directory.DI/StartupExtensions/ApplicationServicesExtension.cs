using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Persistence.Repository;
using Persons.Directory.Persistence.Uow;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>))
                    .AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
