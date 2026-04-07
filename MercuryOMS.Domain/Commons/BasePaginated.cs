public class BasePaginated<T> where T : class
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public BasePaginated(
        IReadOnlyCollection<T> items,
        int pageIndex,
        int pageSize,
        int totalItems)
    {
        Items = items ?? throw new ArgumentNullException(nameof(items));
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalItems = totalItems;

        TotalPages = totalItems == 0
            ? 1
            : (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}