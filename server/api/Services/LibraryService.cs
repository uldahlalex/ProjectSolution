using System.ComponentModel.DataAnnotations;
using api.Etc.Controllers;
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

    public async Task<BookDto> CreateBook(CreateBookRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var book = new Book()
        {
            Pages = dto.Pages,
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title
        };
        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }

    public async Task<BookDto> UpdateBook(UpdateBookRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
    
        var book = ctx.Books.First(b => b.Id == dto.BookIdForLookupReference);
        ctx.Entry(book).Collection(b => b.Authors).Load();
    
        book.Pages = dto.NewPageCout;
        book.Title = dto.NewTitle;
        book.Genre = dto.GenreId != null ? ctx.Genres.First(g => g.Id == dto.GenreId) : null;
    
        book.Authors.Clear();
        dto.AuthorsIds?.ForEach(id => book.Authors.Add(ctx.Authors.First(a => a.Id == id)));
    
        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }

    public async Task<BookDto> DeleteBook(string bookId)
    {
        var book = ctx.Books.First(b => b.Id == bookId);
        ctx.Books.Remove(book);
        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }
}