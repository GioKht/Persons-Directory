using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Middlewares;
using Persons.Directory.Persistence.Repository;
using Persons.Directory.Persistence.Uow;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ValidationActionFilter>()
                    .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                    .AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
