using dataccess;

namespace api.DTOs;

public class BookDto
{
    public BookDto(Book entity)
    {
        Id = entity.Id;
        Title = entity.Title;
        Pages = entity.Pages;
        Createdat = entity.Createdat;
        if (entity.Genre != null)
            Genre = new BooksGenreDto(entity.Genre);
        Authors = entity.Authors?.Select(a => new BooksAuthorDto(a)).ToList() ?? new ();
    }

    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Pages { get; set; }

    public DateTime Createdat { get; set; }

    public BooksGenreDto? Genre { get; set; }

    public List<BooksAuthorDto> Authors { get; set; } = new();



}
public class BooksAuthorDto
{
    public BooksAuthorDto(Author entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Createdat = entity.Createdat;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Createdat { get; set; }

}

public class BooksGenreDto
{
    public BooksGenreDto(Genre entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Createdat = entity.Createdat;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Createdat { get; set; }
}