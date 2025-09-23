using api.DTOs;
using api.DTOs.Requests;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api;

public class LibraryController(ILibraryService libraryService) : ControllerBase
{
    [HttpPost(nameof(GetAuthors))]
    public async Task<List<AuthorDto>> GetAuthors([FromBody]GetAuthorsParameters dto)
    {
        return await libraryService.GetAuthors(dto);
    }

    [HttpPost(nameof(GetBooks))]
    public async Task<List<BookDto>> GetBooks([FromBody]GetBooksParameters dto)
    {
        return await libraryService.GetBooks(dto);
    }

    [HttpPost(nameof(GetGenres))]
    public async Task<List<GenreDto>> GetGenres([FromBody]GetGenresParameters dto)
    {
        return await libraryService.GetGenres(dto);
    }

    [HttpPost(nameof(CreateBook))]
    public async Task<BookDto> CreateBook([FromBody] CreateBookRequestDto dto)
    {
        return await libraryService.CreateBook(dto);
    }

    [HttpPut(nameof(UpdateBook))]
    public async Task<BookDto> UpdateBook([FromBody] UpdateBookRequestDto dto)
    {
        return await libraryService.UpdateBook(dto);
    }

    [HttpDelete(nameof(DeleteBook))]
    public async Task<BookDto> DeleteBook([FromQuery] string bookId)
    {
        return await libraryService.DeleteBook(bookId);
    }

    [HttpPost(nameof(CreateAuthor))]
    public async Task<AuthorDto> CreateAuthor([FromBody] CreateAuthorRequestDto dto)
    {
        return await libraryService.CreateAuthor(dto);
    }

    [HttpPut(nameof(UpdateAuthor))]
    public async Task<AuthorDto> UpdateAuthor([FromBody] UpdateAuthorRequestDto dto)
    {
        return await libraryService.UpdateAuthor(dto);
    }

    [HttpDelete(nameof(DeleteAuthor))]
    public async Task<AuthorDto> DeleteAuthor([FromQuery] string authorId)
    {
        return await libraryService.DeleteAuthor(authorId);
    }

    [HttpPost(nameof(CreateGenre))]
    public async Task<GenreDto> CreateGenre([FromBody] CreateGenreDto dto)
    {
        return await libraryService.CreateGenre(dto);
    }

    [HttpDelete(nameof(DeleteGenre))]
    public async Task<GenreDto> DeleteGenre([FromQuery] string genreId)
    {
        return await libraryService.DeleteGenre(genreId);
    }

    [HttpPut(nameof(UpdateGenre))]
    public async Task<GenreDto> UpdateGenre([FromBody] UpdateGenreRequestDto dto)
    {
        return await libraryService.UpdateGenre(dto);
    }
    
    

}