using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.Shared.Records;

namespace Persons.Directory.Application.PersonManagement.Records;

public record PersonRecord(
    int Id,
    string FirstName,
    string LastName,
    string PersonalId,
    string BirthDate,
    string Image,
    string Gender,
    IEnumerable<RelatedPersonRecord> RelatedPersons, 
    IEnumerable<RelatedPersonRecord> RelatedToPersons, 
    IEnumerable<PhoneNumberModel> PhoneNumbers);
