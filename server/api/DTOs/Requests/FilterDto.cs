namespace api.Services;

/// <summary>
/// Generic filter that can be applied to any property of an entity
/// </summary>
public record FilterDto
{
    /// <summary>
    /// The property name to filter on (e.g., "Name", "Books.Count", "Createdat")
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// The filter operator to apply
    /// </summary>
    public FilterOperator Operator { get; set; }

    /// <summary>
    /// The value to compare against (will be parsed to appropriate type)
    /// </summary>
    public string? Value { get; set; }
}
