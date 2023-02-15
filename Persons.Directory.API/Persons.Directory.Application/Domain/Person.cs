using Persons.Directory.Application.Enums;

namespace Persons.Directory.Application.Domain;

public class Person : Entity
{
    public Person()
    {

    }

    public Person(
        string firstName,
        string lastName,
        string personalId,
        DateOnly birthDate,
        string city,
        string phoneNumber,
        string image,
        Gender gender,
        PhoneNumberType phoneNumberType,
        RelatedType relatedType,
        int? relatedPersonId)
    {
        FirstName = firstName;
        LastName = lastName;
        PersonalId = personalId;
        BirthDate = birthDate;
        City = city;
        PhoneNumber = phoneNumber;
        Image = image;
        RelatedPersonId = relatedPersonId;
        Gender = gender;
        PhoneNumberType = phoneNumberType;
        RelatedType = relatedType;
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string PersonalId { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public string City { get; private set; }

    public string PhoneNumber { get; private set; }

    public int? RelatedPersonId { get; private set; }

    public string Image { get; set; }

    public Gender Gender { get; private set; }

    public PhoneNumberType PhoneNumberType { get; private set; }

    public RelatedType RelatedType { get; private set; }
}
