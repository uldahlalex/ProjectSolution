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