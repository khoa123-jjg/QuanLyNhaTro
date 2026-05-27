using System.Linq.Expressions;

namespace QLNhaTroV7.Extensions;

public static class QueryableExtensions // Giúp LINQ dễ đọc hơn khi lọc dữ liệu.
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> ApplyPaging<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}