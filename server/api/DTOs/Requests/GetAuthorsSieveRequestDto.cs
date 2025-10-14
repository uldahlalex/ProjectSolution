using System.ComponentModel.DataAnnotations;

namespace api.Services;

/// <summary>
/// Request DTO for Sieve-based author filtering and sorting
/// </summary>
public record GetAuthorsSieveRequestDto
{
    /// <summary>
    /// Filters in Sieve format (e.g., "Name@=Bob,Books.Count>5")
    /// </summary>
    public string? Filters { get; set; }

    /// <summary>
    /// Sorts in Sieve format (e.g., "Name,-Books.Count" where - means descending)
    /// </summary>
    public string? Sorts { get; set; }

    /// <summary>
    /// Page number (1-indexed)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [Range(1, 100)]
    public int? PageSize { get; set; }
}
