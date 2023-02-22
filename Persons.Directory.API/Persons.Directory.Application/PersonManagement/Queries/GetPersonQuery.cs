using Application.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.Services;
using Persons.Directory.Application.Shared.Records;

namespace Persons.Directory.Application.PersonManagement.Queries;

public class GetPersonDetailsQueryHandler : IRequestHandler<GetPersonDetailsRequest, GetPersonDetailsResponse>
{
    private readonly IRepository<Person> _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResourceManagerService _resourceManagerService;

    public GetPersonDetailsQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IResourceManagerService resourceManagerService)
        => (_repository, _httpContextAccessor, _resourceManagerService)
        = (unitOfWork.GetRepository<Person>(), httpContextAccessor, resourceManagerService);

    public async Task<GetPersonDetailsResponse> Handle(GetPersonDetailsRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.PersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.Id), true);
        }

        return new GetPersonDetailsResponse
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            PersonalId = person.PersonalId,
            BirthDate = $"{person.BirthDate:dd-MM-yyyy}",
            Image = person.GetImage(_httpContextAccessor),
            Gender = $"{person.Gender}",

            RelatedPersons = person.RelatedPersons.Select(p => new RelatedPersonRecord(
                        p.RelatedPerson.FirstName,
                        p.RelatedPerson.LastName,
                        p.RelatedPerson.PersonalId,
                        $"{p.RelatedPerson.BirthDate:dd-MM-yyyy}",
                        p.RelatedPerson.GetImage(_httpContextAccessor),
                        $"{p.RelatedPerson.Gender}",
                        $"{p.RelatedType}")),

            RelatedToPersons = person.RelatedToPersons.Select(p => new RelatedPersonRecord(
                     p.Person.FirstName,
                     p.Person.LastName,
                     p.Person.PersonalId,
                     $"{p.Person.BirthDate:dd-MM-yyyy}",
                     p.Person.GetImage(_httpContextAccessor),
                     $"{p.Person.Gender}",
                     $"{p.RelatedType}")),

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

    public string Image { get; set; }

    public string Gender { get; set; }

    public IEnumerable<RelatedPersonRecord> RelatedPersons { get; set; }

    public IEnumerable<RelatedPersonRecord> RelatedToPersons { get; set; }

    public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; }

}