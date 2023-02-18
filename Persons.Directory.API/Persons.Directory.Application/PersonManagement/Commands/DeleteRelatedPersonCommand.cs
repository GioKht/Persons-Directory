using MediatR;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Exceptions;
using Persons.Directory.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Persons.Directory.Application.PersonManagement.Commands;

public class DeleteRelatedPersonCommandHandler : IRequestHandler<DeleteRelatedPersonRequest, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Person> _repository;

    public DeleteRelatedPersonCommandHandler(IUnitOfWork unitOfWork)
        => (_unitOfWork, _repository) = (unitOfWork, unitOfWork.GetRepository<Person>());

    public async Task<Unit> Handle(DeleteRelatedPersonRequest request, CancellationToken cancellationToken)
    {
        var person = await _repository.FirstOrDefaultAsync(x => x.Id == request.PersonId &&
                                                                x.RelatedPersonId.HasValue &&
                                                                x.RelatedPersonId.Value == request.RelatedPersonId);

        if (person is null)
        {
            throw new HttpException($"Delete related person failed", HttpStatusCode.NotFound);
        }

        var relatedPerson = await _repository.GetAsync(person.RelatedPersonId.Value);
        if (relatedPerson is null)
        {
            throw new HttpException($"Related person not found by Id: {person.RelatedPersonId.Value}", HttpStatusCode.NotFound);
        }

        person.SetRelatedPersonId();

        var relatedPersons = await _repository.QueryAsync(x => x.RelatedPersonId.HasValue &&
                                                               x.RelatedPersonId.Value == relatedPerson.Id);

        if (relatedPersons.Any())
        {
            await relatedPersons.ForEachAsync(x => x.SetRelatedPersonId());
        }

        _repository.Delete(relatedPerson);
        await _unitOfWork.CommitAsync();

        return new Unit();
    }
}

public class DeleteRelatedPersonRequest : IRequest<Unit>
{
    public int PersonId { get; set; }

    public int RelatedPersonId { get; set; }
}
