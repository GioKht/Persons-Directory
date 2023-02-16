using Persons.Directory.Application.Enums;

namespace Persons.Directory.Application.PersonManagement.Records;

public record PersonRecord(
    int Id,
    string FirstName,
    string LastName,
    string PersonalId,
    string BirthDate,
    string City,
    string PhoneNumber,
    string Image,
    int? RelatedPersonId,
    string Gender,
    string PhoneNumberType,
    string RelatedType);
