using System.ComponentModel.DataAnnotations;
using api.DTOs;
using api.DTOs.Requests;
using dataccess;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

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

        var book = new Book
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

        book.Pages = dto.NewPageCount;
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

    public async Task<AuthorDto> CreateAuthor(CreateAuthorRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var author = new Author
        {
            Id = Guid.NewGuid().ToString(),
            Createdat = DateTime.UtcNow,
            Name = dto.Name
        };
        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    public async Task<AuthorDto> UpdateAuthor(UpdateAuthorRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        var author = ctx.Authors.First(a => a.Id == dto.AuthorIdForLookup);
        ctx.Entry(author).Collection(e => e.Books).Load();
        author.Books.Clear();
        dto.BooksIds.ForEach(id => author.Books.Add(ctx.Books.First(b => b.Id == id)));
        author.Name = dto.NewName;
        ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    public async Task<AuthorDto> DeleteAuthor(string authorId)
    {
        var author = ctx.Authors.First(a => a.Id == authorId);
        ctx.Authors.Remove(author);
        await ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    public async Task<GenreDto> CreateGenre(CreateGenreDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var genre = new Genre
        {
            Id = Guid.NewGuid().ToString(),
            Createdat = DateTime.UtcNow,
            Name = dto.Name
        };
        ctx.Genres.Add(genre);
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }

    public async Task<GenreDto> DeleteGenre(string genreId)
    {
        var genre = ctx.Genres.First(a => a.Id == genreId);
        ctx.Genres.Remove(genre);
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }

    public async Task<GenreDto> UpdateGenre(UpdateGenreRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var genre = ctx.Genres.First(g => g.Id == dto.IdToLookupBy);
        genre.Name = dto.NewName;
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }
}