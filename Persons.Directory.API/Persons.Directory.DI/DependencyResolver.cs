using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application;
using Persons.Directory.Application.Middlewares;
using Persons.Directory.DI.StartupExtensions;
using Persons.Directory.Persistence.Db;

namespace Persons.Directory.DI;

public class DependencyResolver
{
    public static void Resolve(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(nameof(ApplicationDbContext));

        builder.Services
            .AddAppLocalization()
            .AddApplicationServices()
            .AddAppControllers()
            .AddMediatR(typeof(ApplicationProgram))
            .AddFluentValidation()
            .AddApplicationDbContext(connectionString)
            .AddHttpContextAccessor()
            .AddEndpointsApiExplorer();
    }
}
