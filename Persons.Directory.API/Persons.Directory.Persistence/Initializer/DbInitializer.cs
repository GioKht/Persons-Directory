using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Persistence.Db;

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
    }

    private IEnumerable<Person> GetPersons()
    {
        return new List<Person>
        {
            new Person("Giorgi",
            "Khutsishvili",
            "01010101123",
            new DateTime(1980, 01, 01),
            "Tbilisi",
            "577777777",
            "",
            Gender.Male,
            PhoneNumberType.Mobile),

            new Person("Nino",
            "Goderdzishvili",
            "01010101123",
            new DateTime(1980, 01, 01),
            "Tbilisi",
            "577777777",
            "",
            Gender.Female,
            PhoneNumberType.Mobile),

            new Person("John",
            "Doe",
            "577777777",
            new DateTime(1988, 05, 09),
            "Batumi",
            "577329090",
            "",
            Gender.Male,
            PhoneNumberType.Mobile)
        };
    }
}
