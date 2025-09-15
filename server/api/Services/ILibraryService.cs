using api.Etc.Controllers;
using api.Etc.DTOs;

namespace api.Etc;

public interface ILibraryService
{
    Task<List<AuthorDto>> GetAuthors();
    Task<List<BookDto>> GetBooks();
    Task<List<GenreDto>> GetGenres();
    Task<BookDto> CreateBook(CreateBookRequestDto dto);
    Task<BookDto> UpdateBook(UpdateBookRequestDto dto);
    Task<BookDto> DeleteBook(string id);
}