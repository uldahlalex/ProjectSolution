using System.ComponentModel.DataAnnotations;
using api.DTOs;
using api.DTOs.Requests;
using dataccess;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class LibraryService(MyDbContext ctx) : ILibraryService
{
    public async Task<List<Author>> GetAuthors(GetAuthorsRequestDto dto)
    {
        //Vælg hvilke tabeller og entiteter som skal trækkes ud
        IQueryable<Author> query = ctx.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre);
        
        //Filtering
        
        //Ordering / sorting
        if(dto.Ordering == AuthorOrderingOptions.Name)
            query = query.OrderBy(a => a.Name);
        if (dto.Ordering == AuthorOrderingOptions.NumberOfBooksPublished)
            query = query.OrderByDescending(a => a.Books.Count);
        
            //Chunking / pagination
            query = query.Skip(dto.Skip).Take(dto.Take);

            return await query.ToListAsync();
    }

    public Task<List<BookDto>> GetBooks()
    {
        return ctx.Books
            .Include(b => b.Genre)
            .Include(b => b.Authors)
            .Select(b => new BookDto(b)).ToListAsync();
    }

    public Task<List<GenreDto>> GetGenres()
    {
        return ctx.Genres
            .Include(g => g.Books)
            .Select(g => new GenreDto(g)).ToListAsync();
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
        await ctx.Entry(book).Collection(b => b.Authors).LoadAsync();

        book.Pages = dto.NewPageCount;
        book.Title = dto.NewTitle;
        book.Genre = dto.GenreId != null ? ctx.Genres.First(g => g.Id == dto.GenreId) : null;

        book.Authors.Clear();
        dto.AuthorsIds.ForEach(id => book.Authors.Add(ctx.Authors.First(a => a.Id == id)));

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
        await ctx.Entry(author).Collection(e => e.Books).LoadAsync();
        author.Books.Clear();
        dto.BooksIds.ForEach(id => author.Books.Add(ctx.Books.First(b => b.Id == id)));
        author.Name = dto.NewName;
        await ctx.SaveChangesAsync();
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

    public async Task<List<AuthorDto>> GetAuthorDtos()
    {
        return await ctx.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre)
            .Select(a => new AuthorDto(a))
            .ToListAsync();
    }
}

public record GetAuthorsRequestDto
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public AuthorOrderingOptions Ordering { get; set; }
}

public enum AuthorOrderingOptions
{
    Name,
    NumberOfBooksPublished,
}