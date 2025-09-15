using api.Etc.DTOs;

namespace api.Etc;

public interface ILibraryService
{
    Task<List<AuthorDto>> GetAuthors();
    Task<List<BookDto>> GetBooks();
    Task<List<GenreDto>> GetGenres();
}