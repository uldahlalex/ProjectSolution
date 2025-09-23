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
            GenreId = entity.Genreid;
        AuthorsIds = entity.Authors?.Select(a => a.Id).ToList() ?? new List<string>();
    }

    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Pages { get; set; }

    public DateTime Createdat { get; set; }

    public string? GenreId { get; set; }

    public List<string> AuthorsIds { get; set; }
}