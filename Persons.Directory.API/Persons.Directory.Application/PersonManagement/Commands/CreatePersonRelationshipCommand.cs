using MediatR;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Services;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class CreatePersonRelationshipCommandHandler : IRequestHandler<CreatePersonRelationshipRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;
    private readonly IResourceManagerService _resourceManagerService;

    public CreatePersonRelationshipCommandHandler(IUnitOfWork unitOfWork, IResourceManagerService resourceManagerService)
        => (_unitOfWork, _repository, _resourceManagerService)
        = (unitOfWork, unitOfWork.GetRepository<Person>(), resourceManagerService);

    public async Task<Unit> Handle(CreatePersonRelationshipRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.PersonId);
        if (person is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.PersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.PersonId), true);
        }

        var relatedPerson = await _repository.GetAsync(request.RelatedPersonId);
        if (relatedPerson is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.RelatedPersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.RelatedPersonId), true);
        }

        var personRelation = new PersonRelation(person, relatedPerson, request.RelatedType);

        person.RelatedPersons.Add(personRelation);
        relatedPerson.RelatedToPersons.Add(personRelation);

        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class CreatePersonRelationshipRequest : IRequest<Unit>
{
    public int PersonId { get; set; }

    public int RelatedPersonId { get; set; }

    public RelatedType RelatedType { get; set; }
}
