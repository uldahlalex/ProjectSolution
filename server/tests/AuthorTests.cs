using api;
using api.DTOs.Requests;
using api.Services;
using dataccess;
using Microsoft.EntityFrameworkCore;

namespace tests;

public class AuthorTests(ILibraryService libraryService, MyDbContext ctx, ISeeder seeder)
{
    [Fact]
    public async Task UpdateAuthor_CanUpdateAuthorPropertiesAndAddNewBookRelations()
    {
        await seeder.Seed();

        var updateRequest = new UpdateAuthorRequestDto
        {
            AuthorIdForLookup = "1",
            BooksIds = ["1"],
            NewName = "new Name"
        };
        var result = await libraryService.UpdateAuthor(updateRequest);
        Assert.True(result.Books.Count == 1);
        Assert.True(result.Name == updateRequest.NewName);
        Assert.True(ctx.Books.Include(book => book.Authors).First().Authors.First().Name == updateRequest.NewName);
    }
}