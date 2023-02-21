using MediatR;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Constants;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.Services;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;
    private readonly IResourceManagerService _resourceManagerService;

    public DeletePersonCommandHandler(IUnitOfWork unitOfWork, IResourceManagerService resourceManagerService)
        => (_unitOfWork, _repository, _resourceManagerService)
        = (unitOfWork, unitOfWork.GetRepository<Person>(), resourceManagerService);

    public async Task<Unit> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            var message = _resourceManagerService.GetString(ValidationMessages.PersonNotFoundById);
            throw new NotFoundException(string.Format(message, request.Id), true);
        }

        _repository.Delete(person);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class DeletePersonRequest : IRequest<Unit>
{
    public int Id { get; set; }
}
