using api.DTOs;
using api.DTOs.Requests;
using dataccess;
using Sieve.Models;

namespace api.Services;

public interface ILibraryService
{
    
    Task<Book> CreateBook(CreateBookRequestDto dto);
    Task<Book> UpdateBook(UpdateBookRequestDto dto);
    Task<Book> DeleteBook(string id);
    Task<Author> CreateAuthor(CreateAuthorRequestDto dto);
    Task<Author> UpdateAuthor(UpdateAuthorRequestDto dto);
    Task<Author> DeleteAuthor(string authorId);
    Task<Genre> CreateGenre(CreateGenreDto dto);
    Task<Genre> DeleteGenre(string genreId);
    Task<Genre> UpdateGenre(UpdateGenreRequestDto dto);

    Task<List<Author>> GetAuthors(SieveModel sieveModel);
    Task<List<Book>> GetBooks(SieveModel sieveModel);
    Task<List<Genre>> GetGenres(SieveModel sieveModel);
}