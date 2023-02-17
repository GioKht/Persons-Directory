using Application.Infrastructure;
using Application.Infrastructure.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Models;
using Persons.Directory.Application.PersonManagement.Records;

namespace Persons.Directory.Application.PersonManagement.Queries;

public class GetPersonsQueryHandler : IRequestHandler<GetPersonsRequest, GetPersonsResponse>
{
    private readonly IRepository<Person> _repository;

    public GetPersonsQueryHandler(IUnitOfWork unitOfWork) => _repository = unitOfWork.GetRepository<Person>();

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
            .And(request.RelatedPersonId, x => x.RelatedPersonId == request.RelatedPersonId)
            .And(request.Gender, x => x.Gender == request.Gender)
            .And(request.RelatedType, x => x.RelatedType == request.RelatedType);

        var totalCount = baseQuery.Count();

        var persons = await baseQuery.SortAndPage(request).Select(x => new PersonRecord(
           x.Id,
           x.FirstName,
           x.LastName,
           x.PersonalId,
           $"{x.BirthDate:dd-MM-yyyy}",
           x.Image,
           x.RelatedPersonId,
           $"{x.Gender}",
           $"{x.RelatedType}",
           x.PhoneNumbers.Select(p => new PhoneNumberModel
           {
               Number = p.Number,
               NumberType = p.NumberType
           }))).ToListAsync();

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
