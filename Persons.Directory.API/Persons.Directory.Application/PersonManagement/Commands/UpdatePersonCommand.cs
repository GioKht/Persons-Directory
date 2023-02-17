using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using System.Net;
using System.Text.Json.Serialization;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repostiory;

    public UpdatePersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repostiory) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(UpdatePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repostiory.GetAsync(request.Id);

        if (person is null)
        {
            throw new HttpException($"Person not found by Id: {request.Id}", HttpStatusCode.NotFound);
        }

        person.SetValuesToUpdate(request);

        await _repostiory.UpdateAsync(person);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class UpdatePersonRequest : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int CityId { get; set; }

    public IEnumerable<UpdatePhoneNumberModel> PhoneNumbers { get; set; }
}