using dataccess;

namespace api.DTOs;

public class AuthorDto
{
    public AuthorDto(Author entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Createdat = entity.Createdat;
        Books = entity.Books?.Select(b => new BookDto(b)).ToList() ?? new ();
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public List<BookDto> Books { get; set; } = new List<BookDto>();
}

public class AuthorsBookDto {
    public AuthorsBookDto(Book entity)
    {
        Id = entity.Id;
        Title = entity.Title;
        Pages = entity.Pages;
        Createdat = entity.Createdat;
        if (entity.Genre != null)
            Genre = new BooksGenreDto(entity.Genre);
    }

    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Pages { get; set; }

    public DateTime Createdat { get; set; }
    public BooksGenreDto Genre { get; set; }

}