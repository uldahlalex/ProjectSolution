using api.DTOs;
using api.DTOs.Requests;

namespace api.Services;

public interface ILibraryService
{
    Task<List<AuthorDto>> GetAuthors(GetAuthorsParameters dto);
    Task<List<BookDto>> GetBooks(GetBooksParameters dto);
    Task<List<GenreDto>> GetGenres(GetGenresParameters dto);
    Task<BookDto> CreateBook(CreateBookRequestDto dto);
    Task<BookDto> UpdateBook(UpdateBookRequestDto dto);
    Task<BookDto> DeleteBook(string id);
    Task<AuthorDto> CreateAuthor(CreateAuthorRequestDto dto);
    Task<AuthorDto> UpdateAuthor(UpdateAuthorRequestDto dto);
    Task<AuthorDto> DeleteAuthor(string authorId);
    Task<GenreDto> CreateGenre(CreateGenreDto dto);
    Task<GenreDto> DeleteGenre(string genreId);
    Task<GenreDto> UpdateGenre(UpdateGenreRequestDto dto);
}