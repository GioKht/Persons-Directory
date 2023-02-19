using Persons.Directory.Application.Enums;
using Persons.Directory.Application.PersonManagement.Commands;

namespace Persons.Directory.Application.Domain;

public class PersonRelation
{
    public PersonRelation()
    {

    }

    public PersonRelation(Person person, Person relatedPerson, RelatedType relatedType)
    {
        Person = person;
        RelatedPerson = relatedPerson;
        RelatedType = relatedType;
    }

    public int PersonId { get; private set; }

    public int RelatedPersonId { get; private set; }

    public RelatedType RelatedType { get; private set; }

    public virtual Person Person { get; private set; }

    public virtual Person RelatedPerson { get; private set; }
}
