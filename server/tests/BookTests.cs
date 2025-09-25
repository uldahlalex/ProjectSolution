using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using api;
using api.DTOs.Requests;
using api.Services;
using dataccess;

namespace tests;

public class BookTests(MyDbContext ctx, ILibraryService libraryService, ISeeder seeder, ITestOutputHelper outputHelper)
{
    [Fact]
    public async Task GetBooks_CanGetAllBookDtos()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();

        var actual = await libraryService.GetBooks(new GetBooksParameters());

        Assert.Equal(actual.First().Id, ctx.Books.First().Id);
    }

    [Fact]
    public async Task CreateBook_Success()
    {
        var dto = new CreateBookRequestDto
        {
            Pages = 42,
            Title = "Bobs book"
        };

        var actual = await libraryService.CreateBook(dto);

        Assert.True(actual.Id.Length > 10);
        Assert.True(actual.Createdat < DateTime.UtcNow);
        Assert.True(actual.Title == dto.Title);
        Assert.True(actual.Pages == dto.Pages);
        Assert.True(actual.Genre == null );
        Assert.True(actual.Authors.Count == 0);
    }

    [Fact]
    public async Task UpdateBook_IsIdempotent()
    {
        await seeder.Seed();
        var book = ctx.Books.First();
        var author = ctx.Authors.First();
        book.Authors.Add(author);
        author.Books.Add(book);
        await ctx.SaveChangesAsync();

        Assert.True(ctx.Books.First().Authors.First().Id == "1");

        //"update" with same ending state should not have exceptions
        var actual = await libraryService.UpdateBook(new UpdateBookRequestDto()
        {
            AuthorsIds = ["1"],
            BookIdForLookupReference = "1",
            GenreId = null,
            NewPageCount = 42,
            NewTitle = "bla"
        });
        Assert.Contains("1", actual.Authors.Select(s => s.Id));
        Assert.True(ctx.Books.First().Authors.First().Id == "1");
    }

    [Fact]
    public async Task UpdateBook_CanAddGenreAndAuthorAndNewTitleAndPageCount()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();

        var dto = new UpdateBookRequestDto
        {
            NewPageCount = 81,
            NewTitle = "New title",
            GenreId = ctx.Genres.First().Id,
            AuthorsIds = new List<string> { ctx.Authors.First().Id },
            BookIdForLookupReference = ctx.Books.First().Id
        };

        var actual = await libraryService.UpdateBook(dto);
        outputHelper.WriteLine(JsonSerializer.Serialize(actual));
        Assert.True(actual.Genre.Id == ctx.Genres.First().Id);
        Assert.True(actual.Authors.First().Id == ctx.Authors.First().Id);
        Assert.True(actual.Pages == 81);
        Assert.True(actual.Title == "New title");
    }

    [Fact]
    public async Task DeleteBook_CanRemoveExistingBook()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();
        var actual = await libraryService.DeleteBook(ctx.Books.First().Id);
        Assert.True(ctx.Books.Count() == 0);
    }

    [Fact]
    public async Task DeleteBook_ThrowsExceptionIfBookDoesNotExist()
    {
        await Assert.ThrowsAnyAsync<Exception>(async () => await libraryService.DeleteBook("nonexistingID"));
    }

    [Theory]
    [InlineData("okay title", 0)]
    [InlineData("", 42)]
    public async Task Create_ThrowsExceptionIfDataAnnotationValidatorsAreViolated(string title, int pages)
    {
        var dto = new CreateBookRequestDto
        {
            Pages = pages,
            Title = title
        };
        await Assert.ThrowsAnyAsync<ValidationException>(async () => await libraryService.CreateBook(dto));
    }

    [Theory]
    [InlineData("okay title", 0)]
    [InlineData("", 42)]
    public async Task Update_ThrowsExceptionIfDataAnnotationValidatorsAreViolated(string title, int pages)
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();
        //add other things situationally

        var dto = new UpdateBookRequestDto
        {
            NewPageCount = pages,
            NewTitle = title,
            GenreId = ctx.Genres.First().Id,
            BookIdForLookupReference = ctx.Books.First().Id,
            AuthorsIds = ctx.Authors.Select(a => a.Id).ToList()
        };
        await Assert.ThrowsAnyAsync<ValidationException>(async () => await libraryService.UpdateBook(dto));
    }

    [Theory]
    [InlineData(null, null, "non-existing-id")]
    [InlineData(null, "non-existing-id", null)]
    [InlineData("non-existing-id", null, null)]
    public async Task Update_FailsIfEitherEntityDoesNotExist(string? bookId, string? genreId, string? authorsId)
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();

        var dto = new UpdateBookRequestDto
        {
            NewPageCount = 123,
            NewTitle = "New title",
            GenreId = genreId ?? ctx.Genres.First().Id,
            BookIdForLookupReference = bookId ?? ctx.Books.First().Id,
            AuthorsIds = new List<string>
            {
                authorsId ?? ctx.Authors.First().Id
            }
        };

        await Assert.ThrowsAnyAsync<Exception>(async () => await libraryService.UpdateBook(dto));
    }
}