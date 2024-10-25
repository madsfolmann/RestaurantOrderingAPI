using System.Linq.Expressions;

namespace RestaurantOrderingAPI.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> SortByDir<T, TKey>(
            this IQueryable<T> source,
            Expression<Func<T, TKey>> keySelector,
            bool ascending)
        => ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);

        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int pageNumber, int pageSize)
            => source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}