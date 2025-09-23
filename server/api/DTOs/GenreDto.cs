using dataccess;

namespace api.DTOs;

public class GenreDto
{
    public GenreDto(Genre entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Createdat = entity.Createdat;
        Books = entity.Books?.Select(b => new GenresBooksDto(b)).ToList() ?? new();
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Createdat { get; set; }
    public List<GenresBooksDto> Books { get; set; } = new();
}

public class GenresBooksDto
{
    public GenresBooksDto(Book entity)
    {
        Id = entity.Id;
        Title = entity.Title;
        Pages = entity.Pages;
        Createdat = entity.Createdat;
        Authors = entity.Authors?.Select(a => new BooksAuthorDto(a)).ToList() ?? new ();
    }

    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Pages { get; set; }

    public DateTime Createdat { get; set; }

    public List<BooksAuthorDto> Authors { get; set; } = new();
}