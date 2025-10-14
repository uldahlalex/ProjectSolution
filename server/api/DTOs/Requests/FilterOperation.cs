namespace api.Services;

/// <summary>
/// Represents a filter operation with an operator and value
/// </summary>
/// <typeparam name="T">The type of value to filter</typeparam>
public record FilterOperation<T>
{
    /// <summary>
    /// The operation to perform
    /// </summary>
    public FilterOperator Operator { get; set; }

    /// <summary>
    /// The value to compare against
    /// </summary>
    public T? Value { get; set; }
}
