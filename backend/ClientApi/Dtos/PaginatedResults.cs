namespace ClientApi.Dtos;

public record PaginatedResults<T>(
    int Page,
    int PageSize,
    IEnumerable<T> Items
);