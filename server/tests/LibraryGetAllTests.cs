using api.DTOs.Requests;
using api.Services;
using dataccess;

namespace tests;

public class LibraryGetAllTests(ILibraryService libraryService, MyDbContext ctx)
{
    [Fact]
    public async Task GetAuthors_CanGetAllAuthorDtos()
    {
        var author = new Author
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Name = "Bob"
        };
        ctx.Authors.Add(author);
        ctx.SaveChanges();

        var actual = await libraryService.GetAuthors(new GetAuthorsParameters());

        Assert.Equal(actual.First().Id, author.Id);
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

        var actual = await libraryService.GetGenres(new GetGenresParameters());

        Assert.Equal(actual.First().Id, genre.Id);
    }
}