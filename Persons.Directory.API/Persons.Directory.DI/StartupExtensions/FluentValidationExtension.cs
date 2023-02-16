using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application;
using System.Reflection;

namespace Persons.Directory.DI.StartupExtensions;

public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblies(new Assembly[]
        {
            typeof(ApplicationProgram).Assembly
        });

        return services;
    }
}
