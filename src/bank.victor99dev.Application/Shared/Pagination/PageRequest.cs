namespace bank.victor99dev.Application.Shared.Pagination;

public sealed record PageRequest(int Page = 1, int PageSize = 10)
{
    public int Skip => (Page - 1) * PageSize;
}