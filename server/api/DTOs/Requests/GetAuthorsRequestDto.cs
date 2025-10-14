using System.ComponentModel.DataAnnotations;

namespace api.Services;

/// <summary>
/// Request DTO for getting authors with filtering, sorting, and pagination
/// </summary>
public record GetAuthorsRequestDto : PaginatedRequestDto
{
    // Inherits Skip, Take, SortBy, SortAscending, and Filters from PaginatedRequestDto

    // Optional: Keep backward compatibility with enum-based ordering
    /// <summary>
    /// (Deprecated) Use SortBy and SortAscending instead. Predefined sorting options.
    /// </summary>
    public AuthorOrderingOptions? Ordering { get; set; }
}