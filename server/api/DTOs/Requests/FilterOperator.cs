namespace api.Services;

/// <summary>
/// Filter operations that can be applied to field values
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Exact match (=)
    /// </summary>
    Equals,

    /// <summary>
    /// Not equal (!=)
    /// </summary>
    NotEquals,

    /// <summary>
    /// Contains substring (for strings)
    /// </summary>
    Contains,

    /// <summary>
    /// Starts with substring (for strings)
    /// </summary>
    StartsWith,

    /// <summary>
    /// Ends with substring (for strings)
    /// </summary>
    EndsWith,

    /// <summary>
    /// Greater than (&gt;)
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Less than (&lt;)
    /// </summary>
    LessThan,

    /// <summary>
    /// Greater than or equal (&gt;=)
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Less than or equal (&lt;=)
    /// </summary>
    LessThanOrEqual
}
