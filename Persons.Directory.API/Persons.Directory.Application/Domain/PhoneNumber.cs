using Persons.Directory.Application.Enums;
using Persons.Directory.Application.PersonManagement.Models;

namespace Persons.Directory.Application.Domain;

public class PhoneNumber : Entity
{
    public PhoneNumber()
    {

    }

    public PhoneNumber(PhoneNumberModel model)
    {
        Number = model.Number;
        NumberType = model.NumberType;
        CreatedDate = DateTime.Now;
    }

    public void Update(UpdatePhoneNumberModel model)
    {
        Number = model.Number;
        NumberType = model.NumberType;
        UpdatedDate = DateTime.Now;
    }

    public string Number { get; private set; }

    public PhoneNumberType NumberType { get; private set; }

    public int PersonId { get; private set; }

    public virtual Person Person { get; private set; }
}
