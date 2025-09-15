namespace api.Etc.DTOs;

public class AuthorDto
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public virtual ICollection<BookDto> Books { get; set; } = new List<BookDto>();

}