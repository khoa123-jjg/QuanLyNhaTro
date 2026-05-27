namespace QLNhaTroV7.ViewModels.Common;

public class PagedResult<T> // Dùng chung cho phân trang.
{
    public List<T> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages
    {
        get
        {
            if (PageSize <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling((double)TotalItems / PageSize);
        }
    }

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
