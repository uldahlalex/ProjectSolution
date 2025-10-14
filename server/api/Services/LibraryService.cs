using System.ComponentModel.DataAnnotations;
using api.DTOs;
using api.DTOs.Requests;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace api.Services;

public class LibraryService(MyDbContext ctx, ISieveProcessor sieveProcessor) : ILibraryService
{
    

    public Task<List<Book>> GetBooks(SieveModel sieveModel)
    {
        IQueryable<Book> query =  ctx.Books;

        query = sieveProcessor.Apply(sieveModel, query);
        
            return query
            .Include(b => b.Genre)
            .Include(b => b.Authors)
            .ToListAsync();
    }

    public Task<List<Genre>> GetGenres(SieveModel sieveModel)
    {
        IQueryable<Genre> query =  ctx.Genres;
            
        query = sieveProcessor.Apply(sieveModel, query);
        
        return query.Include(g => g.Books)
            .ThenInclude(b => b.Authors)
            .ToListAsync();
    }

    public async Task<Book> CreateBook(CreateBookRequestDto dto)
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
        return book;
    }

    public async Task<Book> UpdateBook(UpdateBookRequestDto dto)
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
        return book;
    }

    public async Task<Book> DeleteBook(string bookId)
    {
        var book = ctx.Books.First(b => b.Id == bookId);
        ctx.Books.Remove(book);
        await ctx.SaveChangesAsync();
        return book;
    }

    public async Task<Author> CreateAuthor(CreateAuthorRequestDto dto)
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
        return author;
    }

    public async Task<Author> UpdateAuthor(UpdateAuthorRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);
        var author = ctx.Authors.First(a => a.Id == dto.AuthorIdForLookup);
        await ctx.Entry(author).Collection(e => e.Books).LoadAsync();
        author.Books.Clear();
        dto.BooksIds.ForEach(id => author.Books.Add(ctx.Books.First(b => b.Id == id)));
        author.Name = dto.NewName;
        await ctx.SaveChangesAsync();
        return author;
    }

    public async Task<Author> DeleteAuthor(string authorId)
    {
        var author = ctx.Authors.First(a => a.Id == authorId);
        ctx.Authors.Remove(author);
        await ctx.SaveChangesAsync();
        return author;
    }

    public async Task<Genre> CreateGenre(CreateGenreDto dto)
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
        return genre;
    }

    public async Task<Genre> DeleteGenre(string genreId)
    {
        var genre = ctx.Genres.First(a => a.Id == genreId);
        ctx.Genres.Remove(genre);
        await ctx.SaveChangesAsync();
        return genre;
    }

    public async Task<Genre> UpdateGenre(UpdateGenreRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var genre = ctx.Genres.First(g => g.Id == dto.IdToLookupBy);
        genre.Name = dto.NewName;
        await ctx.SaveChangesAsync();
        return genre;
    }
    

    public async Task<List<Author>> GetAuthors(SieveModel sieveModel)
    {
        IQueryable<Author> query = ctx.Authors;
        query = sieveProcessor.Apply(sieveModel, query);
        query = query.Include(a => a.Books)
            .ThenInclude(b => b.Genre);
        return query.ToList();
    }

    // public async Task<List<Author>> GetAuthorsSieve(GetAuthorsSieveRequestDto dto)
    // {
    //     Validator.ValidateObject(dto, new ValidationContext(dto), true);
    //
    //     // Start with base query WITHOUT includes (to allow Books.Count filtering/sorting)
    //     IQueryable<Author> query = ctx.Authors;
    //
    //     // Convert our DTO to Sieve's SieveModel
    //     var sieveModel = new SieveModel
    //     {
    //         Filters = dto.Filters,
    //         Sorts = dto.Sorts,
    //         Page = dto.Page,
    //         PageSize = dto.PageSize
    //     };
    //
    //     // Apply Sieve filtering, sorting, and pagination BEFORE includes
    //     query = sieveProcessor.Apply(sieveModel, query);
    //
    //     // NOW include related data after filtering/sorting/pagination
    //     query = query
    //         .Include(a => a.Books)
    //         .ThenInclude(b => b.Genre);
    //
    //     return await query.ToListAsync();
    // }
}