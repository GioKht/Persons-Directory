using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class AddRelatedPersonHandler : IRequestHandler<AddRelatedPersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public AddRelatedPersonHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(AddRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.PersonId);

        if (person is null)
        {
            throw new HttpException($"Add related person failed, person not found by Id: {request.PersonId}", HttpStatusCode.NotFound);
        }

        var relatedPerson = await _repository.GetAsync(request.RelatedPersonId);

        if (relatedPerson is null)
        {
            throw new HttpException($"Add related person failed, relatedPerson not found by Id: {request.RelatedPersonId}", HttpStatusCode.NotFound);
        }

        relatedPerson.SetRelatedPersonId(person.Id);
        relatedPerson.SetRelatedType(request.RelatedType);

        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class AddRelatedPersonRequest : IRequest<Unit>
{
    public int PersonId { get; set; }

    public int RelatedPersonId { get; set; }

    public RelatedType RelatedType { get; set; }
}
