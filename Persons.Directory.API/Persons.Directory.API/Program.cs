using Microsoft.AspNetCore.Mvc;
using Persons.Directory.API.Configurations;
using Persons.Directory.API.SwaggerSupport;
using Persons.Directory.Application.Middlewares;
using Persons.Directory.DI;
using Persons.Directory.Persistence.Initializer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

Logger.Configure(builder);
DependencyResolver.Resolve(builder);

builder.Services.AddSwaggerGen(c=>
{
    c.OperationFilter<AddAcceptLanguageHeaderParameter>();
});

var app = builder.Build();

var dbInitializer = new DbInitializer();
await dbInitializer.Seed(app.Services);

app.UseErrorHandlingMiddleware();
app.UseMiddleware<AcceptLanguageMiddleware>();

//app.UseRequestLocalization();

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
