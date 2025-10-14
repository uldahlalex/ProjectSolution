using System.Linq.Expressions;
using dataccess;

namespace api.Services;

public static class AuthorQueryExtensions
{
    public static IQueryable<Author> ApplyFilters(
        this IQueryable<Author> query,
        AuthorFilters? filters)
    {
        if (filters == null) return query;

        if (filters.Name != null)
            query = query.ApplyStringFilter(a => a.Name, filters.Name);

        if (filters.BooksPublished != null)
            query = query.ApplyComparableFilter(a => a.Books.Count, filters.BooksPublished);

        if (filters.GenreName != null)
            query = query.Where(a => a.Books.Any(b =>
                b.Genre != null && ApplyStringFilterExpression(b.Genre.Name, filters.GenreName)));

        if (filters.CreatedAt != null)
            query = query.ApplyComparableFilter(a => a.Createdat, filters.CreatedAt);

        return query;
    }

    private static IQueryable<T> ApplyStringFilter<T>(
        this IQueryable<T> query,
        Expression<Func<T, string>> selector,
        FilterOperation<string> filter)
    {
        if (filter.Value == null) return query;

        return filter.Operator switch
        {
            FilterOperator.Equals => query.Where(Combine(selector, v => v == filter.Value)),
            FilterOperator.NotEquals => query.Where(Combine(selector, v => v != filter.Value)),
            FilterOperator.Contains => query.Where(Combine(selector, v => v.Contains(filter.Value))),
            FilterOperator.StartsWith => query.Where(Combine(selector, v => v.StartsWith(filter.Value))),
            FilterOperator.EndsWith => query.Where(Combine(selector, v => v.EndsWith(filter.Value))),
            _ => query
        };
    }

    private static IQueryable<T> ApplyComparableFilter<T, TValue>(
        this IQueryable<T> query,
        Expression<Func<T, TValue>> selector,
        FilterOperation<TValue> filter) where TValue : IComparable<TValue>
    {
        if (filter.Value == null) return query;

        return filter.Operator switch
        {
            FilterOperator.Equals => query.Where(Combine(selector, v => v.Equals(filter.Value))),
            FilterOperator.NotEquals => query.Where(Combine(selector, v => !v.Equals(filter.Value))),
            FilterOperator.GreaterThan => query.Where(Combine(selector, v => v.CompareTo(filter.Value) > 0)),
            FilterOperator.LessThan => query.Where(Combine(selector, v => v.CompareTo(filter.Value) < 0)),
            FilterOperator.GreaterThanOrEqual => query.Where(Combine(selector, v => v.CompareTo(filter.Value) >= 0)),
            FilterOperator.LessThanOrEqual => query.Where(Combine(selector, v => v.CompareTo(filter.Value) <= 0)),
            _ => query
        };
    }

    private static bool ApplyStringFilterExpression(string value, FilterOperation<string> filter)
    {
        if (filter.Value == null) return true;

        return filter.Operator switch
        {
            FilterOperator.Equals => value == filter.Value,
            FilterOperator.NotEquals => value != filter.Value,
            FilterOperator.Contains => value.Contains(filter.Value),
            FilterOperator.StartsWith => value.StartsWith(filter.Value),
            FilterOperator.EndsWith => value.EndsWith(filter.Value),
            _ => true
        };
    }

    private static Expression<Func<T, bool>> Combine<T, TValue>(
        Expression<Func<T, TValue>> selector,
        Expression<Func<TValue, bool>> predicate)
    {
        var parameter = selector.Parameters[0];
        var body = Expression.Invoke(predicate, selector.Body);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
