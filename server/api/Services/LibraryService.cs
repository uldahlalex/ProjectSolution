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
    public async Task<List<Author>> GetAuthors(GetAuthorsRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        //Select data to extract
        IQueryable<Author> query = ctx.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre);

        //Filtering - using generic filter system
        query = query.ApplyFilters(dto.Filters);

        //Ordering / sorting - support both old and new system
        if (dto.Ordering.HasValue)
        {
            // Backward compatibility with enum-based ordering
            query = dto.Ordering.Value switch
            {
                AuthorOrderingOptions.Name => query.OrderBy(a => a.Name),
                AuthorOrderingOptions.NameDescending => query.OrderByDescending(a => a.Name),
                AuthorOrderingOptions.NumberOfBooksPublished => query
                    .OrderByDescending(a => a.Books.Count)
                    .ThenBy(a => a.Name),
                _ => query.OrderBy(a => a.Id)
            };
        }
        else
        {
            // New generic sorting system
            query = query.ApplySorting(dto.SortBy ?? "Id", dto.SortAscending);
        }

        //Chunking / pagination
        query = query.Skip(dto.Skip).Take(dto.Take);

        //return some POCO
        var list = await query.ToListAsync();

        return list;
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

    public async Task<List<Author>> GetAuthorsSieveBasic(SieveModel sieveModel)
    {
        IQueryable<Author> authors = ctx.Authors;
        sieveProcessor.Apply(sieveModel, authors);
        return authors.ToList();
    }

    public async Task<List<Author>> GetAuthorsSieve(GetAuthorsSieveRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        // Start with base query WITHOUT includes (to allow Books.Count filtering/sorting)
        IQueryable<Author> query = ctx.Authors;

        // Convert our DTO to Sieve's SieveModel
        var sieveModel = new SieveModel
        {
            Filters = dto.Filters,
            Sorts = dto.Sorts,
            Page = dto.Page,
            PageSize = dto.PageSize
        };

        // Apply Sieve filtering, sorting, and pagination BEFORE includes
        query = sieveProcessor.Apply(sieveModel, query);

        // NOW include related data after filtering/sorting/pagination
        query = query
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre);

        return await query.ToListAsync();
    }
}