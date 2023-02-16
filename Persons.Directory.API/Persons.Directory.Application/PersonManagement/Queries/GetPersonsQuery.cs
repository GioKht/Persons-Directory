using Application.Infrastructure;
using Application.Infrastructure.Enums;
using MediatR;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Records;

namespace Persons.Directory.Application.PersonManagement.Queries
{
    public class GetPersonsQueryHandler : IRequestHandler<GetPersonsRequest, GetPersonsResponse>
    {
        private readonly IRepository<Person> _repository;

        public GetPersonsQueryHandler(IUnitOfWork unitOfWork)
        {
            _repository = unitOfWork.GetRepository<Person>();
        }

        public async Task<GetPersonsResponse> Handle(GetPersonsRequest request, CancellationToken cancellationToken)
        {
            request.SearchTerm = request.SearchTerm?.Trim().ToLower();

            var baseQuery = await _repository.QueryAsync();

            var totalCount = baseQuery.Count();

            var persons = baseQuery.SortAndPage(request).Select(
                x => new PersonRecord(
                x.Id,
                x.FirstName,
                x.LastName,
                x.PersonalId,
                x.BirthDate.ToString("dd-MM-yyyy"),
                x.City,
                x.PhoneNumber,
                x.Image,
                x.RelatedPersonId,
                x.Gender.ToString(),
                x.PhoneNumberType.ToString(),
                x.RelatedType.ToString()))
                .ToList();

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
}
