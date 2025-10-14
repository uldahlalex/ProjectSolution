namespace api.Services;

public record AuthorFilters
{
    /// <summary>
    /// Filter by author name with flexible operators (Equals, Contains, StartsWith, etc.)
    /// </summary>
    public FilterOperation<string>? Name { get; set; }

    /// <summary>
    /// Filter by number of books published with comparison operators (Equals, GreaterThan, LessThan, etc.)
    /// </summary>
    public FilterOperation<int>? BooksPublished { get; set; }

    /// <summary>
    /// Filter authors who have books in specific genre with flexible operators
    /// </summary>
    public FilterOperation<string>? GenreName { get; set; }

    /// <summary>
    /// Filter by author creation date with comparison operators
    /// </summary>
    public FilterOperation<DateTime>? CreatedAt { get; set; }
}