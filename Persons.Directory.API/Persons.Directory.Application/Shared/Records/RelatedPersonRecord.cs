namespace Persons.Directory.Application.Shared.Records;

public record  RelatedPersonRecord(
    string FirstName,
    string LastName,
    string PersonalId,
    string BirthDate,
    string Image,
    string Gender,
    string relatedType);
