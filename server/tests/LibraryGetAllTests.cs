using api.Etc;
using dataccess;

namespace tests;

public class LibraryGetAllTests(ILibraryService libraryService, MyDbContext ctx)
{
    [Fact]
    public async Task GetAuthors_CanGetAllAuthorDtos()
    {
        var author = new Author()
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Name = "Bob"
        };
        ctx.Authors.Add(author);
        ctx.SaveChanges();
        
        var actual = await libraryService.GetAuthors();
        
        Assert.Equal(actual.First().Id, author.Id);
    }
    
    

    
    [Fact]
    public async Task GetGenres_CanGetAllGenreDtos()
    {
        var genre = new Genre()
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