using Application.Infrastructure;
using Application.Infrastructure.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.PersonManagement.Records;
using Persons.Directory.Application.Shared.Records;

namespace Persons.Directory.Application.PersonManagement.Queries;

public class GetPersonsQueryHandler : IRequestHandler<GetPersonsRequest, GetPersonsResponse>
{
    private readonly IRepository<Person> _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetPersonsQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        => (_repository, _httpContextAccessor) = (unitOfWork.GetRepository<Person>(), httpContextAccessor);

    public async Task<GetPersonsResponse> Handle(GetPersonsRequest request, CancellationToken cancellationToken)
    {
        request.SearchTerm = request.SearchTerm?.Trim().ToLower();

        var baseQuery = _repository.Query()
            .And(request.SearchTerm, x => x.FirstName.Contains(request.SearchTerm) ||
                                         x.LastName.Contains(request.SearchTerm) ||
                                         x.PersonalId.Contains(request.SearchTerm))
            .And(request.BirthDate, x => x.BirthDate == request.BirthDate)
            .And(request.PhoneNumber, x => x.PhoneNumbers.Any(e => e.Number == request.PhoneNumber))
            .And(request.PhoneNumberType, x => x.PhoneNumbers.Any(e => e.NumberType == request.PhoneNumberType))
            .And(request.Gender, x => x.Gender == request.Gender);

        var totalCount = baseQuery.Count();

        var persons = await baseQuery
            .SortAndPage(request)
            .Select(x => new PersonRecord(
                x.Id,
                x.FirstName,
                x.LastName,
                x.PersonalId,
                $"{x.BirthDate:dd-MM-yyyy}",
                x.GetImage(_httpContextAccessor),
                $"{x.Gender}",

                x.RelatedPersons.Select(p => new RelatedPersonRecord(
                        p.RelatedPerson.FirstName,
                        p.RelatedPerson.LastName,
                        p.RelatedPerson.PersonalId,
                        $"{p.RelatedPerson.BirthDate:dd-MM-yyyy}",
                        p.RelatedPerson.GetImage(_httpContextAccessor),
                        $"{p.RelatedPerson.Gender}",
                        $"{p.RelatedType}")),

                x.RelatedToPersons.Select(p => new RelatedPersonRecord(
                        p.Person.FirstName,
                        p.Person.LastName,
                        p.Person.PersonalId,
                        $"{p.Person.BirthDate:dd-MM-yyyy}",
                        p.Person.GetImage(_httpContextAccessor),
                        $"{p.Person.Gender}",
                        $"{p.RelatedType}")),

                x.PhoneNumbers.Select(p => new PhoneNumberModel
                {
                    Number = p.Number,
                    NumberType = p.NumberType
                })
            ))
            .ToListAsync();

        return new GetPersonsResponse
        {
            TotalCount = totalCount,
            Items = persons
        };
    }
}

public class GetPersonsRequest : IQuery<GetPersonsResponse>, IPagedQuery
{
    public string? SearchTerm { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? CityId { get; set; }

    public string? PhoneNumber { get; set; }

    public int? RelatedPersonId { get; set; }

    public Gender? Gender { get; set; }

    public PhoneNumberType? PhoneNumberType { get; set; }

    public RelatedType? RelatedType { get; set; }

    public int? PageSize { get; set; }

    public int? Page { get; set; }

    public SortOrder? SortOrder { get; set; }

    public string? SortBy { get; set; }
}

public class GetPersonsResponse : IPagedQueryResult
{
    public int TotalCount { get; set; }

    public IEnumerable<PersonRecord> Items { get; set; }
}
