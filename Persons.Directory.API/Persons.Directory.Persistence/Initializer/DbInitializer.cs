using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Models;
using Persons.Directory.Application.PersonManagement.Commands;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Persistence.Db;
using System.Text.Json;

namespace Persons.Directory.Persistence.Initializer;

public class DbInitializer
{
    public async Task Seed(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        var persons = GetPersons();

        var existingIds = await context.Set<Person>().Select(p => p.PersonalId).ToListAsync();
        var newPersons = persons.Where(p => !existingIds.Contains(p.PersonalId)).ToList();

        if (newPersons.Any())
        {
            await context.Set<Person>().AddRangeAsync(newPersons);
            await context.SaveChangesAsync();
        }

        var citiesList = await context.Set<City>().ToListAsync();

        if (!citiesList.Any())
        {
            using var jsonStream = File.OpenRead("CitiesData.json");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var result = await JsonSerializer.DeserializeAsync<CityResultModel>(jsonStream, options);

            result.Cities.ForEach(x=> x.SetCreateDate());

            await context.Set<City>().AddRangeAsync(result.Cities);
            await context.SaveChangesAsync();
        }
    }

    private IEnumerable<Person> GetPersons()
    {
        return new List<Person>
        {
            new Person(new CreatePersonRequest 
            {
                FirstName = "Giorgi",
                LastName = "Khutsishvili",
                PersonalId = "01010101123",
                BirthDate = new DateTime(1980, 01, 01),
                CityId = 1,
                Gender = Gender.Male,
                PhoneNumbers = new List<PhoneNumberModel>()
                {
                    new PhoneNumberModel
                    {
                        Number = "57701490",
                        NumberType = PhoneNumberType.Mobile,
                    },
                    new PhoneNumberModel
                    {
                        Number = "+0123456",
                        NumberType = PhoneNumberType.Office
                    }
                }
            }),

            new Person(new CreatePersonRequest
            {
                FirstName = "Nino",
                LastName = "Goderdzishvili",
                PersonalId = "01010101123",
                BirthDate = new DateTime(1980, 01, 01),
                CityId = 2,
                Gender = Gender.Female,
                PhoneNumbers = new List<PhoneNumberModel>()
                {
                    new PhoneNumberModel
                    {
                        Number = "577223344",
                        NumberType = PhoneNumberType.Mobile
                    },
                    new PhoneNumberModel
                    {
                        Number = "+333123123123",
                        NumberType = PhoneNumberType.Office
                    }
                }
            }),

            new Person(new CreatePersonRequest
            {
                FirstName = "John",
                LastName = "Doe",
                PersonalId = "01010101123",
                BirthDate = new DateTime(1988, 05, 09),
                CityId = 3,
                Gender = Gender.Male,
                PhoneNumbers = new List<PhoneNumberModel>()
                {
                    new PhoneNumberModel
                    {
                        Number = "577443355",
                        NumberType = PhoneNumberType.Mobile
                    },
                    new PhoneNumberModel
                    {
                        Number = "+1111111111111111",
                        NumberType = PhoneNumberType.Office
                    }
                }
            })
        };
    }
}
