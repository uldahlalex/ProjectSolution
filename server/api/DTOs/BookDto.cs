namespace api.Etc.DTOs;

public class BookDto
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Pages { get; set; }

    public DateTime? Createdat { get; set; }
    
    public virtual GenreDto? Genre { get; set; }

    public virtual ICollection<string> AuthorsIds { get; set; } = new List<string>();

}