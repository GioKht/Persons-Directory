using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Persons.Directory.DI.StartupExtensions
{
    public static class AppLocalizationExtension
    {
        public static IServiceCollection AddAppLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources")
                .Configure<RequestLocalizationOptions>(options =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("ka-GE")
                    };

                    options.DefaultRequestCulture = new RequestCulture("en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

            return services;
        }
    }
}
