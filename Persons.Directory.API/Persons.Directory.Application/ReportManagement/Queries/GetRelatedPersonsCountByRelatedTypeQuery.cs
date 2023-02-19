using Application.Infrastructure;
using Application.Infrastructure.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Enums;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Application.PersonManagement.Records;
using Persons.Directory.Application.Shared.Records;

namespace Persons.Directory.Application.ReportManagement.Queries
{
    public class GetRelatedPersonsCountByRelatedTypeQueryHandler : IRequestHandler<GetRelatedPersonsRequest, GetRelatedPersonsResponse>
    {
        private readonly IRepository<Person> _repository;

        public GetRelatedPersonsCountByRelatedTypeQueryHandler(IUnitOfWork unitOfWork)
            => _repository = unitOfWork.GetRepository<Person>();

        public async Task<GetRelatedPersonsResponse> Handle(GetRelatedPersonsRequest request, CancellationToken cancellationToken)
        {
            var baseQuery = _repository.Query()
                .And(request.SearchTerm, x => x.FirstName.Contains(request.SearchTerm) ||
                                              x.LastName.Contains(request.SearchTerm) ||
                                              x.PersonalId.Contains(request.SearchTerm));

            var persons = await baseQuery
                .SortAndPage(request)
                .ToListAsync();

            var items = persons.GroupBy(p => new { p.Id, p.FirstName, p.LastName, p.PersonalId, p.RelatedPersons, p.RelatedToPersons })
                .Select(g => new GetRelatedPersonsResponseModel
                {
                    Id = g.Key.Id,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    PersonalId = g.Key.PersonalId,

                    RelatedPersons = g.Key.RelatedPersons.Select(p => new RelatedPersonRecord(
                        p.RelatedPerson.FirstName,
                        p.RelatedPerson.LastName,
                        p.RelatedPerson.PersonalId,
                        $"{p.RelatedPerson.BirthDate:dd-MM-yyyy}",
                        null,
                        $"{p.RelatedPerson.Gender}",
                        $"{p.RelatedType}")),

                    RelatedToPersons = g.Key.RelatedToPersons.Select(p => new RelatedPersonRecord(
                        p.Person.FirstName,
                        p.Person.LastName,
                        p.Person.PersonalId,
                        $"{p.Person.BirthDate:dd-MM-yyyy}",
                        null,
                        $"{p.Person.Gender}",
                        $"{p.RelatedType}")),

                    RelatedTypeCounts = g.SelectMany(x => x.RelatedPersons)
                            .GroupBy(rp => rp.RelatedType)
                            .Select(gr => new RelatedTypeCount
                            {
                                Type = gr.Key,
                                Count = gr.Count()
                            })
                            .ToList()
                });

            return new GetRelatedPersonsResponse
            {
                TotalCount = items.Count(),
                Items = items
            };
        }
    }

    public class GetRelatedPersonsRequest : IQuery<GetRelatedPersonsResponse>, IPagedQuery
    {
        public string? SearchTerm { get; set; }

        public int? PageSize { get; set; }

        public int? Page { get; set; }

        public SortOrder? SortOrder { get; set; }

        public string? SortBy { get; set; }
    }

    public class GetRelatedPersonsResponse : IPagedQueryResult
    {
        public int TotalCount { get; set; }

        public IEnumerable<GetRelatedPersonsResponseModel> Items { get; set; }

    }

    public class GetRelatedPersonsResponseModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PersonalId { get; set; }

        public IEnumerable<RelatedPersonRecord> RelatedPersons { get; set; }

        public IEnumerable<RelatedPersonRecord> RelatedToPersons { get; set; }

        public IEnumerable<RelatedTypeCount> RelatedTypeCounts { get; set; }
    }

    public class RelatedPersonDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PersonalId { get; set; }

        public RelatedType RelatedType { get; set; }
    }

    public class RelatedTypeCount
    {
        public RelatedType Type { get; set; }

        public int Count { get; set; }
    }
}


//var result = await persons
//    .SortAndPage(request)
//    .GroupBy(p => new { p.Id, p.FirstName, p.LastName })
//    .Select(g => new GetRelatedPersonsResponseModel
//    {
//        Id = g.Key.Id,
//        FirstName = g.Key.FirstName,
//        LastName = g.Key.LastName,
//        RelatedTypeCounts = g.SelectMany(x => x.RelatedPersons)
//            .Where(rp => rp.RelatedType.HasValue)
//            .AsEnumerable()
//            .GroupBy(rp => rp.RelatedType.Value)
//            .Select(gr => new RelatedTypeCount
//            {
//                Type = gr.Key,
//                Count = gr.Count()
//            })
//            .ToList()
//    })
//    .ToListAsync();