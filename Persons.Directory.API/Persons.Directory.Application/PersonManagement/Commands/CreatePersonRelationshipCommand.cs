using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class CreatePersonRelationshipCommandHandler : IRequestHandler<CreatePersonRelationshipRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public CreatePersonRelationshipCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(CreatePersonRelationshipRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.PersonId);

        if (person is null)
        {
            throw new HttpException($"Person not found by Id: {request.PersonId}", HttpStatusCode.NotFound);
        }

        var relatedPerson = await _repository.GetAsync(request.RelatedPersonId);

        if (relatedPerson is null)
        {
            throw new HttpException($"RelatedPerson not found by Id: {request.RelatedPersonId}", HttpStatusCode.NotFound);
        }

        person.SetRelatedPersonId(relatedPerson.Id);
        person.SetRelatedType(request.RelatedType);

        relatedPerson.SetRelatedPersonId(person.Id);
        relatedPerson.SetRelatedType(request.RelatedType);

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
