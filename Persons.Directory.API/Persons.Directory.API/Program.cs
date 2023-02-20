using Microsoft.AspNetCore.Mvc;
using Persons.Directory.API.Configurations;
using Persons.Directory.Application.Infrastructure;
using Persons.Directory.DI;
using Persons.Directory.Persistence.Initializer;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<ValidationActionFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ValidationActionFilter));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

Logger.Configure(builder);
DependencyResolver.Resolve(builder);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var dbInitializer = new DbInitializer();
await dbInitializer.Seed(app.Services);

app.UseErrorHandlingMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
