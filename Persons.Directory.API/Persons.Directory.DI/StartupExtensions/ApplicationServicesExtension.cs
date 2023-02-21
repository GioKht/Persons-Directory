using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Middlewares;
using Persons.Directory.Application.Services;
using Persons.Directory.Persistence.Repository;
using Persons.Directory.Persistence.Uow;
using System.Reflection;
using System.Resources;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ValidationActionFilter>()
                    .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                    .AddScoped<IUnitOfWork, UnitOfWork>()
                    .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                    .AddSingleton<IResourceManagerService>(provider =>
                    {
                        var assembly = typeof(SharedResource).Assembly;
                        var resourceManager = new ResourceManager("Persons.Directory.Application.Resources.SharedResource", assembly);
                        return new ResourceManagerService(resourceManager);
                    });

            return services;
        }
    }
}
