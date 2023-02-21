using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application.Middlewares;
using System.Text.Json.Serialization;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class AppControllersExtension
    {
        public static IServiceCollection AddAppControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ValidationActionFilter));
            })
            .AddDataAnnotationsLocalization()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return services;
        }
    }
}
