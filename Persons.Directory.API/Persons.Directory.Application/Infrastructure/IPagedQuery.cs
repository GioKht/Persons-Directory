using Application.Infrastructure.Enums;

namespace Application.Infrastructure;

public interface IPagedQuery
{
    string? SearchTerm { get; set; }

    int? PageSize { get; set; }

    int? Page { get; set; }

    public SortOrder? SortOrder { get; set; }

    public string? SortBy { get; set; }
}

public interface IPagedQueryResult
{
    int TotalCount { get; set; }
}
