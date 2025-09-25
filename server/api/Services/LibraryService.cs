using System.ComponentModel.DataAnnotations;
using api.DTOs;
using api.DTOs.Requests;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace api.Services;

public class LibraryService(MyDbContext ctx) : ILibraryService
{
    public async Task<List<AuthorDto>> GetAuthors(GetAuthorsParameters dto)
    {
        var query = ctx.Authors
            .Include(a => a.Books)
            .ThenInclude(b => b.Genre)
            .AsQueryable();

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(dto.FullTextSearchFilter))
        {
            query = query.Where(a =>
                EF.Functions.ToTsVector("english", a.Name + " " + (a.Name ?? ""))
                    .Matches(EF.Functions.PlainToTsQuery("english", dto.FullTextSearchFilter)));
        }

        // Apply ordering
        query = dto.OrderBy switch
        {
            AuthorOrderBy.Name => dto.Descending ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name),
            AuthorOrderBy.CreatedAt => dto.Descending ? query.OrderByDescending(a => a.Createdat) : query.OrderBy(a => a.Createdat),
            AuthorOrderBy.NumberOfPublishedBooks => dto.Descending ? query.OrderByDescending(a => a.Books.Count) : query.OrderBy(a => a.Books.Count),
            _ => dto.Descending ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name)
        };

        // Apply pagination and execute
        return await query
            .Skip(dto.StartAt)
            .Take(dto.Limit)
            .Select(a => new AuthorDto(a))
            .ToListAsync();
    }

    public async Task<List<BookDto>> GetBooks(GetBooksParameters dto)
    {
        var query = ctx.Books
            .Include(b => b.Genre)
            .Include(b => b.Authors)
            .AsQueryable();

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(dto.FullTextSearchFilter))
        {
            query = query.Where(b =>
                EF.Functions.ToTsVector("english", b.Title + " " + (b.Title ?? ""))
                    .Matches(EF.Functions.PlainToTsQuery("english", dto.FullTextSearchFilter)));
        }

        // Apply page filtering
        if (dto.BookPagesMinimum > 0)
        {
            query = query.Where(b => b.Pages >= dto.BookPagesMinimum);
        }
        if (dto.BookPagesMaximum < int.MaxValue)
        {
            query = query.Where(b => b.Pages <= dto.BookPagesMaximum);
        }

        // Apply ordering
        query = dto.OrderBy switch
        {
            BookOrderBy.Title => dto.Descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            BookOrderBy.Author => dto.Descending ? query.OrderByDescending(b => b.Authors.FirstOrDefault().Name) : query.OrderBy(b => b.Authors.FirstOrDefault().Name),
            BookOrderBy.PageCount => dto.Descending ? query.OrderByDescending(b => b.Pages) : query.OrderBy(b => b.Pages),
            BookOrderBy.CreatedAt => dto.Descending ? query.OrderByDescending(b => b.Createdat) : query.OrderBy(b => b.Createdat),
            BookOrderBy.Genre => dto.Descending ? query.OrderByDescending(b => b.Genre.Name) : query.OrderBy(b => b.Genre.Name),
            _ => dto.Descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title)
        };

        // Apply pagination and execute
        return await query
            .Skip(dto.StartAt)
            .Take(dto.Limit)
            .Select(b => new BookDto(b))
            .ToListAsync();
    }

    public async Task<List<GenreDto>> GetGenres(GetGenresParameters dto)
    {
        var query = ctx.Genres
            .Include(g => g.Books)
            .AsQueryable();

        // Apply filtering
        if (!string.IsNullOrWhiteSpace(dto.FullTextSearchFilter))
        {
            query = query.Where(g =>
                EF.Functions.ToTsVector("english", g.Name + " " + (g.Name ?? ""))
                    .Matches(EF.Functions.PlainToTsQuery("english", dto.FullTextSearchFilter)));
        }

        // Apply ordering
        query = dto.OrderBy switch
        {
            GenreOrderBy.Name => dto.Descending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name),
            GenreOrderBy.CreatedAt => dto.Descending ? query.OrderByDescending(g => g.Createdat) : query.OrderBy(g => g.Createdat),
            GenreOrderBy.NumberOfBooksInGenre => dto.Descending ? query.OrderByDescending(g => g.Books.Count) : query.OrderBy(g => g.Books.Count),
            _ => dto.Descending ? query.OrderByDescending(g => g.Name) : query.OrderBy(g => g.Name)
        };

        // Apply pagination and execute
        return await query
            .Skip(dto.StartAt)
            .Take(dto.Limit)
            .Select(g => new GenreDto(g))
            .ToListAsync();
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
}