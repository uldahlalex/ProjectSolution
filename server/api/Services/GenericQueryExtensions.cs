using System.Linq.Expressions;
using System.Reflection;

namespace api.Services;

/// <summary>
/// Generic query extensions that work with any entity type
/// </summary>
public static class GenericQueryExtensions
{
    /// <summary>
    /// Apply filters to a queryable entity
    /// </summary>
    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        List<FilterDto>? filters)
    {
        if (filters == null || filters.Count == 0) return query;

        foreach (var filter in filters)
        {
            query = query.ApplyFilter(filter);
        }

        return query;
    }

    /// <summary>
    /// Apply a single filter to a queryable entity
    /// </summary>
    private static IQueryable<T> ApplyFilter<T>(
        this IQueryable<T> query,
        FilterDto filter)
    {
        if (string.IsNullOrWhiteSpace(filter.PropertyName) || filter.Value == null)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetPropertyExpression(parameter, filter.PropertyName);

        if (property == null) return query;

        var propertyType = property.Type;
        var constant = ConvertValue(filter.Value, propertyType);

        if (constant == null) return query;

        var comparison = BuildComparison(property, filter.Operator, constant, propertyType);

        if (comparison == null) return query;

        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
        return query.Where(lambda);
    }

    /// <summary>
    /// Apply sorting to a queryable entity
    /// </summary>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        string? sortBy,
        bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = GetPropertyExpression(parameter, sortBy);

        if (property == null) return query;

        var lambda = Expression.Lambda(property, parameter);

        var methodName = ascending ? "OrderBy" : "OrderByDescending";
        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
    }

    /// <summary>
    /// Get property expression supporting nested properties (e.g., "Books.Count")
    /// </summary>
    private static Expression? GetPropertyExpression(Expression parameter, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        Expression? expression = parameter;

        foreach (var propName in properties)
        {
            var type = expression.Type;
            PropertyInfo? propertyInfo;

            // Handle special case for Count on collections
            if (propName.Equals("Count", StringComparison.OrdinalIgnoreCase) &&
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                propertyInfo = type.GetProperty("Count");
            }
            else
            {
                propertyInfo = type.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }

            if (propertyInfo == null) return null;

            expression = Expression.Property(expression, propertyInfo);
        }

        return expression;
    }

    /// <summary>
    /// Convert string value to target type
    /// </summary>
    private static Expression? ConvertValue(string value, Type targetType)
    {
        try
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            object? convertedValue = underlyingType.Name switch
            {
                nameof(String) => value,
                nameof(Int32) => int.Parse(value),
                nameof(Int64) => long.Parse(value),
                nameof(Double) => double.Parse(value),
                nameof(Decimal) => decimal.Parse(value),
                nameof(Boolean) => bool.Parse(value),
                nameof(DateTime) => DateTime.Parse(value),
                nameof(Guid) => Guid.Parse(value),
                _ => Convert.ChangeType(value, underlyingType)
            };

            return Expression.Constant(convertedValue, targetType);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Build comparison expression based on operator
    /// </summary>
    private static Expression? BuildComparison(
        Expression property,
        FilterOperator op,
        Expression constant,
        Type propertyType)
    {
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // String operations
        if (underlyingType == typeof(string))
        {
            return op switch
            {
                FilterOperator.Equals => Expression.Equal(property, constant),
                FilterOperator.NotEquals => Expression.NotEqual(property, constant),
                FilterOperator.Contains => Expression.Call(property, "Contains", null, constant),
                FilterOperator.StartsWith => Expression.Call(property, "StartsWith", null, constant),
                FilterOperator.EndsWith => Expression.Call(property, "EndsWith", null, constant),
                _ => null
            };
        }

        // Comparable operations (int, DateTime, etc.)
        return op switch
        {
            FilterOperator.Equals => Expression.Equal(property, constant),
            FilterOperator.NotEquals => Expression.NotEqual(property, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
            FilterOperator.LessThan => Expression.LessThan(property, constant),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            _ => null
        };
    }
}
