using MediatR;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Services;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class DeleteRelatedPersonCommandHandler : IRequestHandler<DeleteRelatedPersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;
    private readonly IResourceManagerService _resourceManagerService;

    public DeleteRelatedPersonCommandHandler(IUnitOfWork unitOfWork, IResourceManagerService resourceManagerService)
        => (_unitOfWork, _repository, _resourceManagerService)
        = (unitOfWork, unitOfWork.GetRepository<Person>(), resourceManagerService);

    public async Task<Unit> Handle(DeleteRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.PersonId);
        if (person is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.DeleteRelatedPersonFailed);
            throw new NotFoundException(message, true);
        }

        var relatedPerson = person.RelatedPersons.FirstOrDefault(x => x.RelatedPersonId == request.RelatedPersonId);
        if (relatedPerson is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.RelatedPersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.RelatedPersonId), true);
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
