using Application.Infrastructure;
using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.PersonManagement.Records;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Queries;

public class GetPersonDetailsQueryHandler : IRequestHandler<GetPersonDetailsRequest, GetPersonDetailsResponse>
{
    private readonly IRepository<Person> _repository;

    public GetPersonDetailsQueryHandler(IUnitOfWork unitOfWork) => _repository = unitOfWork.GetRepository<Person>();

    public async Task<GetPersonDetailsResponse> Handle(GetPersonDetailsRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            throw new HttpException($"Person not found by Id: {request.Id}", HttpStatusCode.NotFound);
        }

        var dbRelatedPersons = await _repository.QueryAsync(x => x.RelatedPersonId == person.Id);

        var relatedPersons = Enumerable.Empty<PersonRecord>();

        if (dbRelatedPersons.Any())
        {
            relatedPersons = dbRelatedPersons.Select(x => new PersonRecord(
               x.Id,
               x.FirstName,
               x.LastName,
               x.PersonalId,
               $"{x.BirthDate:dd-MM-yyyy}",
               x.City,
               x.Image,
               x.RelatedPersonId,
               $"{x.Gender}",
               $"{x.RelatedType}",
               x.PhoneNumbers.Select(p => new PhoneNumberModel
               {
                   Number = p.Number,
                   NumberType = p.NumberType
               })));
        }

        return new GetPersonDetailsResponse
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            PersonalId = person.PersonalId,
            BirthDate = $"{person.BirthDate:dd-MM-yyyy}",
            RelatedPersonId = person.RelatedPersonId,
            Image = person.Image,
            Gender = $"{person.Gender}",
            RelatedType = $"{person.RelatedType}",
            RelatedPersons = relatedPersons,
            PhoneNumbers = person.PhoneNumbers.Select(p => new PhoneNumberModel
            {
                Number = p.Number,
                NumberType = p.NumberType
            })
        };
    }
}

public class GetPersonDetailsRequest : IQuery<GetPersonDetailsResponse>
{
    public int Id { get; set; }
}

public class GetPersonDetailsResponse
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PersonalId { get; set; }

    public string BirthDate { get; set; }

    public int? RelatedPersonId { get; set; }

    public string Image { get; set; }

    public string Gender { get; set; }

    public string RelatedType { get; set; }

    public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; }

    public IEnumerable<PersonRecord> RelatedPersons { get; set; }
}