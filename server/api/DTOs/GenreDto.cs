namespace api.Etc.DTOs;

public class GenreDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? Createdat { get; set; }
    public List<string> Books { get; set; } = new List<string>();
}