using System.ComponentModel.DataAnnotations;

namespace api.Services;

/// <summary>
/// Base request DTO for paginated queries with filtering and sorting
/// </summary>
public record PaginatedRequestDto
{
    /// <summary>
    /// Number of items to skip for pagination
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }

    /// <summary>
    /// Number of items to take (page size)
    /// </summary>
    [Range(1, 100)]
    public int Take { get; set; }

    /// <summary>
    /// Property name to sort by (e.g., "Name", "Createdat")
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: true for ascending, false for descending
    /// </summary>
    public bool SortAscending { get; set; } = true;

    /// <summary>
    /// List of filters to apply (AND logic between filters)
    /// </summary>
    public List<FilterDto>? Filters { get; set; }
}
