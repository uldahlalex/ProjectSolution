using dataccess;

namespace api.Etc;

public class Seeder(MyDbContext ctx) : ISeeder
{
    public async Task Seed()
    {   
        var author = new Author()
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Name = "Bob"
        };
        ctx.Authors.Add(author);
        ctx.SaveChanges();
        var book = new Book()
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Pages = 42,
            Title = "Bobs book"
        };
        ctx.Books.Add(book);
        ctx.SaveChanges();
        var genre = new Genre()
        {
            Createdat = DateTime.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Name = "thriller"
        };
        ctx.Genres.Add(genre);
        ctx.SaveChanges();
        
    }
}

public interface ISeeder
{
    public Task Seed();
}