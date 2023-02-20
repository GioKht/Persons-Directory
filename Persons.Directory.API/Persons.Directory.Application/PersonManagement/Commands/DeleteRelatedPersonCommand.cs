using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class DeleteRelatedPersonCommandHandler : IRequestHandler<DeleteRelatedPersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public DeleteRelatedPersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(DeleteRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.PersonId);
        if (person is null)
        {
            throw new BadRequestException($"Delete related person failed", HttpStatusCode.NotFound);
        }

        var relatedPerson = person.RelatedPersons.FirstOrDefault(x => x.RelatedPersonId == request.RelatedPersonId);
        if (relatedPerson is null)
        {
            throw new BadRequestException($"Related person not found by Id: {request.RelatedPersonId}", HttpStatusCode.NotFound);
        }

        person.RelatedPersons.Remove(relatedPerson);

        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class DeleteRelatedPersonRequest : IRequest<Unit>
{
    public int PersonId { get; set; }

    public int RelatedPersonId { get; set; }
}
