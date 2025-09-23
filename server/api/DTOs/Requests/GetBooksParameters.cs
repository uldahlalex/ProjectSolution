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

public record BaseGetParameters
{
    public BaseGetParameters(bool descending, int startAt, int limit, string? fullTextSearchFilter = null)
    {
        Descending = descending;
        if(limit==0) 
            limit = 25;
        FullTextSearchFilter = fullTextSearchFilter;
    }

    public bool Descending { get; set; } = false;
    public int StartAt { get; set; } = 0;
    public int Limit { get; set; } = 5;
    public string? FullTextSearchFilter { get; set; }
}
public record GetBooksParameters : BaseGetParameters
{
    public GetBooksParameters(BookOrderBy orderBy, int bookPagesMinimum, int bookPagesMaximum, bool descending, int startAt, int limit, string? fullTextSearchFilter = null) : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
        BookPagesMinimum = bookPagesMinimum;
        BookPagesMaximum = bookPagesMaximum;
    }

    public BookOrderBy OrderBy { get; set; }
    public int BookPagesMinimum { get; set; }
    public int BookPagesMaximum { get; set; }
}


public record GetAuthorsParameters : BaseGetParameters
{
    public GetAuthorsParameters(AuthorOrderBy orderBy, bool descending, int startAt, int limit, string? fullTextSearchFilter = null) : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
    }

    public AuthorOrderBy OrderBy { get; set; }
    
}

public record GetGenresParameters : BaseGetParameters
{
    public GetGenresParameters(GenreOrderBy orderBy, bool descending, int startAt, int limit, string? fullTextSearchFilter = null) : base(descending, startAt, limit, fullTextSearchFilter)
    {
        OrderBy = orderBy;
    }

    public GenreOrderBy OrderBy { get; set; }
    
}