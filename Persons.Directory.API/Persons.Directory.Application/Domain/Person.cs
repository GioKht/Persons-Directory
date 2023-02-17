using Persons.Directory.Application.Enums;
using Persons.Directory.Application.PersonManagement.Commands;

namespace Persons.Directory.Application.Domain;

public class Person : Entity
{
    public Person()
    {
        PhoneNumbers = new List<PhoneNumber>();
    }

    public Person(CreatePersonRequest request)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        PersonalId = request.PersonalId;
        BirthDate = request.BirthDate;
        CityId = request.CityId;
        Image = request.Image;
        RelatedPersonId = request.RelatedPersonId;
        Gender = request.Gender;
        RelatedType = request.RelatedType;
        CreatedDate = DateTime.Now;

        if (request.PhoneNumbers.Any())
        {
            var phoneNumbers = request.PhoneNumbers.Select(number => new PhoneNumber(number));
            PhoneNumbers = phoneNumbers.ToList();
        }
    }

    public void SetValuesToUpdate(UpdatePersonRequest request)
    {
        FirstName = request.FirstName;
        LastName = request.LastName;
        CityId = request.CityId;
        UpdatedDate = DateTime.Now;

        if (request.PhoneNumbers.Any())
        {
            var phoneNumbersList = new List<PhoneNumber>();

            foreach (var phoneNumber in request.PhoneNumbers)
            {
                var dbPhoneNumber = PhoneNumbers.FirstOrDefault(x => x.Id == phoneNumber.Id);

                if (dbPhoneNumber is not null)
                {
                    dbPhoneNumber.Update(phoneNumber);
                }
            }
        }
    }

    public void SetRelatedPersonId()
    {
        RelatedPersonId = null;
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string PersonalId { get; private set; }

    public DateTime BirthDate { get; private set; }

    public int CityId { get; private set; }

    public int? RelatedPersonId { get; private set; }

    public string? Image { get; private set; }

    public Gender Gender { get; private set; }

    public RelatedType? RelatedType { get; private set; }

    public virtual ICollection<PhoneNumber> PhoneNumbers { get; private set; }
}
