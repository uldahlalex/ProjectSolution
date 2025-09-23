using dataccess;

namespace api.DTOs;

public class AuthorDto
{
    public AuthorDto(Author entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        Createdat = entity.Createdat;
        BookIds = entity.Books?.Select(b => b.Id).ToList() ?? new List<string>();
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public List<string> BookIds { get; set; } = new List<string>();
}