using Persons.Directory.Persistence.Db;
using Serilog;

namespace Persons.Directory.API.Configurations;

public class Logger
{
    public static void Configure(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.MSSqlServer(
                builder.Configuration.GetConnectionString(nameof(ApplicationDbContext)),
                tableName: "Logs",
                autoCreateSqlTable: true)
            .CreateLogger();
    }
}
