using System.Text.Json;
using System.Text.Json.Serialization;
using api;
using api.Services;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit.Internal;

namespace tests;

public class LibraryGetAllTests(ILibraryService libraryService, MyDbContext ctx, ITestOutputHelper outputHelper)
{
    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 6)]
    public async Task GetAuthors_CanPaginateAndOrderByNameAlphabetically(int skip, int take)
    {
        await new SeederWithRelations(ctx).Seed();
        
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.Name,
            Skip = skip,
            Take = take
        });

        outputHelper.WriteLine(JsonSerializer.Serialize(actual, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        Assert.Equal(skip+"", actual.First().Id);
        Assert.Equal(take, actual.Count);
        Assert.Equal("Bob_"+((skip+take)-1), actual.Last().Name);
    }

    [Fact]
    public async Task GetAuthors_CanOrderByNumberOfBooksPublished()
    {
        await new SeederWithRelations(ctx).Seed();

        ctx.Books.Add(new Book()
        {
            Createdat = DateTime.UtcNow,
            Pages = 42,
            Title = "Booky book",
            Id = "2"
        });
        await ctx.SaveChangesAsync();

        var authorToHave2Books = ctx.Authors.Include(a => a.Books).First(a => a.Id == "7");
        authorToHave2Books.Books.Clear();
        ctx.Books.ForEach((b) => authorToHave2Books.Books.Add(b));
        await ctx.SaveChangesAsync();
        
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.NumberOfBooksPublished,
            Skip = 0,
            Take = 3
        });

        outputHelper.WriteLine(JsonSerializer.Serialize(actual, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = true
        }));

        Assert.Equal("7", actual.First().Id); 
        Assert.Equal("0", actual[1].Id); 
        Assert.Equal("1", actual[2].Id); 
    }


    [Fact]
    public async Task GetGenres_CanGetAllGenreDtos()
    {
        var genre = new Genre
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Name = "thriller"
        };
        ctx.Genres.Add(genre);
        ctx.SaveChanges();

        var actual = await libraryService.GetGenres();

        Assert.Equal(actual.First().Id, genre.Id);
    }
}