using api.Etc.DTOs;
using dataccess;
using Microsoft.EntityFrameworkCore;

namespace api.Etc;

public class LibraryService(MyDbContext ctx) : ILibraryService
{
    public Task<List<AuthorDto>> GetAuthors()
    {
        return ctx.Authors.Select(a => new AuthorDto(a)).ToListAsync();
    }

    public Task<List<BookDto>> GetBooks()
    {
        return ctx.Books.Select(b => new BookDto(b)).ToListAsync();
    }

    public Task<List<GenreDto>> GetGenres()
    {
        return ctx.Genres.Select(g => new GenreDto(g)).ToListAsync();
    }
}