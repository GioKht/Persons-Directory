using Persons.Directory.Application.Enums;
using Persons.Directory.Application.PersonManagement.Models;

namespace Persons.Directory.Application.PersonManagement.Records;

public record PersonRecord(
    int Id,
    string FirstName,
    string LastName,
    string PersonalId,
    string BirthDate,
    string Image,
    int? RelatedPersonId,
    string Gender,
    string RelatedType,
    IEnumerable<PhoneNumberModel> PhoneNumbers);
