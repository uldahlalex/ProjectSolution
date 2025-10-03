using api.DTOs;
using api.DTOs.Requests;
using dataccess;

namespace api.Services;

public interface ILibraryService
{
    Task<List<Author>> GetAuthors(int skip = 0, int take = Int32.MaxValue);
    Task<List<BookDto>> GetBooks();
    Task<List<GenreDto>> GetGenres();
    Task<BookDto> CreateBook(CreateBookRequestDto dto);
    Task<BookDto> UpdateBook(UpdateBookRequestDto dto);
    Task<BookDto> DeleteBook(string id);
    Task<AuthorDto> CreateAuthor(CreateAuthorRequestDto dto);
    Task<AuthorDto> UpdateAuthor(UpdateAuthorRequestDto dto);
    Task<AuthorDto> DeleteAuthor(string authorId);
    Task<GenreDto> CreateGenre(CreateGenreDto dto);
    Task<GenreDto> DeleteGenre(string genreId);
    Task<GenreDto> UpdateGenre(UpdateGenreRequestDto dto);
    Task<List<AuthorDto>> GetAuthorDtos();
}