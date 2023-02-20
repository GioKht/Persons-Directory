using MediatR;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using System.Net;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public DeletePersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(DeletePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.GetAsync(request.Id);

        if (person is null)
        {
            throw new BadRequestException($"Person not found by Id: {request.Id}", HttpStatusCode.NotFound);
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
