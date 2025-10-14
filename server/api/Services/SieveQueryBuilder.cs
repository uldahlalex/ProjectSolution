using System.Linq.Expressions;
using System.Text;
using Sieve.Models;

namespace api.Services;

/// <summary>
/// Type-safe builder for constructing Sieve query strings
/// </summary>
/// <typeparam name="T">The entity type to build queries for</typeparam>
public class SieveQueryBuilder<T> where T : class
{
    private readonly List<string> _filters = new();
    private readonly List<string> _sorts = new();
    private int? _page;
    private int? _pageSize;

    /// <summary>
    /// Add a filter using equals operator (==)
    /// </summary>
    public SieveQueryBuilder<T> FilterEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}=={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using not equals operator (!=)
    /// </summary>
    public SieveQueryBuilder<T> FilterNotEquals<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}!={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using contains operator (@=)
    /// </summary>
    public SieveQueryBuilder<T> FilterContains<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}@={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using starts with operator (_=)
    /// </summary>
    public SieveQueryBuilder<T> FilterStartsWith<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}_={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using greater than operator (>)
    /// </summary>
    public SieveQueryBuilder<T> FilterGreaterThan<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}>{value}");
        return this;
    }

    /// <summary>
    /// Add a filter using less than operator (<)
    /// </summary>
    public SieveQueryBuilder<T> FilterLessThan<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}<{value}");
        return this;
    }

    /// <summary>
    /// Add a filter using greater than or equal operator (>=)
    /// </summary>
    public SieveQueryBuilder<T> FilterGreaterThanOrEqual<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}>={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using less than or equal operator (<=)
    /// </summary>
    public SieveQueryBuilder<T> FilterLessThanOrEqual<TProp>(Expression<Func<T, TProp>> property, TProp value)
    {
        var propertyName = GetPropertyName(property);
        _filters.Add($"{propertyName}<={value}");
        return this;
    }

    /// <summary>
    /// Add a filter using a custom property name (for mapped properties like BooksCount)
    /// </summary>
    public SieveQueryBuilder<T> FilterByName(string propertyName, string operatorSymbol, object value)
    {
        _filters.Add($"{propertyName}{operatorSymbol}{value}");
        return this;
    }

    /// <summary>
    /// Add ascending sort for a property
    /// </summary>
    public SieveQueryBuilder<T> SortBy<TProp>(Expression<Func<T, TProp>> property)
    {
        var propertyName = GetPropertyName(property);
        _sorts.Add(propertyName);
        return this;
    }

    /// <summary>
    /// Add descending sort for a property
    /// </summary>
    public SieveQueryBuilder<T> SortByDescending<TProp>(Expression<Func<T, TProp>> property)
    {
        var propertyName = GetPropertyName(property);
        _sorts.Add($"-{propertyName}");
        return this;
    }

    /// <summary>
    /// Add sort using a custom property name (for mapped properties like BooksCount)
    /// </summary>
    public SieveQueryBuilder<T> SortByName(string propertyName, bool descending = false)
    {
        _sorts.Add(descending ? $"-{propertyName}" : propertyName);
        return this;
    }

    /// <summary>
    /// Set the page number for pagination
    /// </summary>
    public SieveQueryBuilder<T> Page(int page)
    {
        _page = page;
        return this;
    }

    /// <summary>
    /// Set the page size for pagination
    /// </summary>
    public SieveQueryBuilder<T> PageSize(int pageSize)
    {
        _pageSize = pageSize;
        return this;
    }

    /// <summary>
    /// Build the Filters query string component
    /// </summary>
    public string BuildFiltersString()
    {
        return _filters.Any() ? string.Join(",", _filters) : string.Empty;
    }

    /// <summary>
    /// Build the Sorts query string component
    /// </summary>
    public string BuildSortsString()
    {
        return _sorts.Any() ? string.Join(",", _sorts) : string.Empty;
    }

    /// <summary>
    /// Build a complete SieveModel object
    /// </summary>
    public SieveModel BuildSieveModel()
    {
        return new SieveModel
        {
            Filters = BuildFiltersString(),
            Sorts = BuildSortsString(),
            Page = _page,
            PageSize = _pageSize
        };
    }

    /// <summary>
    /// Build the complete query string for use in HTTP requests
    /// </summary>
    public string BuildQueryString()
    {
        var parts = new List<string>();

        if (_filters.Any())
        {
            parts.Add($"filters={Uri.EscapeDataString(string.Join(",", _filters))}");
        }

        if (_sorts.Any())
        {
            parts.Add($"sorts={Uri.EscapeDataString(string.Join(",", _sorts))}");
        }

        if (_page.HasValue)
        {
            parts.Add($"page={_page.Value}");
        }

        if (_pageSize.HasValue)
        {
            parts.Add($"pageSize={_pageSize.Value}");
        }

        return parts.Any() ? string.Join("&", parts) : string.Empty;
    }

    /// <summary>
    /// Extract property name from expression, handling nested properties
    /// </summary>
    private static string GetPropertyName<TProp>(Expression<Func<T, TProp>> property)
    {
        if (property.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        if (property.Body is UnaryExpression unaryExpression &&
            unaryExpression.Operand is MemberExpression operand)
        {
            return operand.Member.Name;
        }

        throw new ArgumentException($"Expression '{property}' does not refer to a property.");
    }

    /// <summary>
    /// Create a new builder instance for fluent API
    /// </summary>
    public static SieveQueryBuilder<T> Create() => new();
}
