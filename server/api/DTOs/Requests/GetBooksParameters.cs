namespace api.DTOs.Requests;

public enum BookOrderBy
{
    Title,
    Author,
    PageCount,
    CreatedAt,
    Genre
}

public enum AuthorOrderBy
{
    Name,
    CreatedAt,
    NumberOfPublishedBooks
}

public enum GenreOrderBy
{
    Name,
    CreatedAt,
    NumberOfBooksInGenre
}





public record GetGenresParameters : BaseGetParameters
{
    public GetGenresParameters(GenreOrderBy orderBy = GenreOrderBy.Name, bool descending = false, int startAt = 0, int limit = 25, string? fullTextSearchFilter = null) 
        : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
    }

    public GenreOrderBy OrderBy { get; set; }
    
}

public record BaseGetParameters
{
    // Parameterless constructor for model binding
    public BaseGetParameters() { }

    // Optional constructor with parameters
    public BaseGetParameters(bool descending = false, int startAt = 0, int limit = 25, string? fullTextSearchFilter = null)
    {
        Descending = descending;
        StartAt = startAt;
        Limit = limit;
        FullTextSearchFilter = fullTextSearchFilter;
    }

    public bool Descending { get; set; } = false;
    public int StartAt { get; set; } = 0;
    public int Limit { get; set; } = 25;
    public string? FullTextSearchFilter { get; set; }
}

public record GetAuthorsParameters : BaseGetParameters
{
    public GetAuthorsParameters() { }

    public GetAuthorsParameters(AuthorOrderBy orderBy = AuthorOrderBy.Name, bool descending = false, int startAt = 0, int limit = 25, string? fullTextSearchFilter = null) 
        : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
    }

    public AuthorOrderBy OrderBy { get; set; } = AuthorOrderBy.Name;
}

public record GetBooksParameters : BaseGetParameters
{
    public GetBooksParameters() { }

    public GetBooksParameters(BookOrderBy orderBy = BookOrderBy.Title, bool descending = false, int startAt = 0, int limit = 25, string? fullTextSearchFilter = null, int bookPagesMinimum = 0, int bookPagesMaximum = int.MaxValue) 
        : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
        BookPagesMinimum = bookPagesMinimum;
        BookPagesMaximum = bookPagesMaximum;
    }

    public BookOrderBy OrderBy { get; set; } = BookOrderBy.Title;
    public int BookPagesMinimum { get; set; } = 0;
    public int BookPagesMaximum { get; set; } = int.MaxValue;
}