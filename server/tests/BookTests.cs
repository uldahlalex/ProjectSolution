using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using api.Etc;
using api.Etc.Controllers;
using api.Etc.DTOs;
using dataccess;
using Microsoft.AspNetCore.Components.Web;

namespace tests;

public class BookTests(MyDbContext ctx, ILibraryService libraryService, ISeeder seeder, ITestOutputHelper outputHelper)
{
    [Fact]
    public async Task GetBooks_CanGetAllBookDtos()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();
        
        var actual = await libraryService.GetBooks();
        
        Assert.Equal(actual.First().Id, ctx.Books.First().Id);
    }

    [Fact]
    public async Task CreateBook_Success()
    {
        var dto = new CreateBookRequestDto()
        {
            Pages = 42,
            Title = "Bobs book"
        };
        
        var actual = await libraryService.CreateBook(dto);
        
        Assert.True(actual.Id.Length>10);
        Assert.True(actual.Createdat<DateTime.UtcNow);
        Assert.True(actual.Title == dto.Title);
        Assert.True(actual.Pages == dto.Pages);
        Assert.True(actual.Genre == null);
        Assert.True(actual.AuthorsIds.Count==0);
    }

    [Fact]
    public async Task UpdateBook_CanAddGenreAndAuthorAndNewTitleAndPageCount()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();

        var dto = new UpdateBookRequestDto()
        {
            NewPageCout = 81,
            NewTitle = "New title",
            GenreId = ctx.Genres.First().Id,
            AuthorsIds = new List<string>() { ctx.Authors.First().Id },
            BookIdForLookupReference = ctx.Books.First().Id
        };

        var actual = await libraryService.UpdateBook(dto);
        outputHelper.WriteLine(JsonSerializer.Serialize(actual));
        Assert.True(actual.Genre.Id == ctx.Genres.First().Id);
        Assert.True(actual.AuthorsIds.First() == ctx.Authors.First().Id);
        Assert.True(actual.Pages == 81);
        Assert.True(actual.Title == "New title");

    }
    
    [Fact]
    public async Task DeleteBook_CanRemoveExistingBook()
    {
        //Existing data is using the "seeder" with 1 book, 1 author and 1 genre without any relations
        await seeder.Seed();
        var actual = await libraryService.DeleteBook(ctx.Books.First().Id);
        Assert.True(ctx.Books.Count()==0);
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
        var dto = new CreateBookRequestDto()
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
        
        var dto = new UpdateBookRequestDto()
        {
            NewPageCout = pages,
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
        
        var dto = new UpdateBookRequestDto()
        {
            NewPageCout = 123,
            NewTitle = "New title",
            GenreId = genreId ?? ctx.Genres.First().Id,
            BookIdForLookupReference = bookId ?? ctx.Books.First().Id,
            AuthorsIds = new List<string>()
            {
                authorsId ?? ctx.Authors.First().Id
            } 
        };

        await Assert.ThrowsAnyAsync<Exception>(async () => await libraryService.UpdateBook(dto));
    }
    
}