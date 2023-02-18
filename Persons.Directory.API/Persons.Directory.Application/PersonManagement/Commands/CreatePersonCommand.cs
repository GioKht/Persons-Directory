using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public CreatePersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(CreatePersonRequest request, CancellationToken cancellationToken)
    {
        Person person = new(request);

        await _repository.InsertAsync(person);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class CreatePersonRequest : IRequest<Unit>
{
    public CreatePersonRequest()
    {

    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string PersonalId { get; set; }

    public DateTime BirthDate { get; set; }

    public int CityId { get; set; }

    public Gender Gender { get; set; }

    public IEnumerable<PhoneNumberModel> PhoneNumbers { get; set; }
}