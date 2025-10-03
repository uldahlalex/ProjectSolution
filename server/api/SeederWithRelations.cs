using dataccess;
using Microsoft.EntityFrameworkCore;

namespace api;

public class SeederWithRelations(MyDbContext ctx) : ISeeder
{
    public async Task Seed()
    {
        var books = await ctx.Books.Include(b => b.Authors).ToListAsync();
        foreach (var b in books)
        {
            b.Authors.Clear();
        }
        await ctx.SaveChangesAsync();

        // 2. Delete books (references genres and authors)
        ctx.Books.RemoveRange(ctx.Books);
        await ctx.SaveChangesAsync();

        // 3. Delete authors (no longer referenced)
        ctx.Authors.RemoveRange(ctx.Authors);
        await ctx.SaveChangesAsync();

        // 4. Delete genres (no longer referenced)  
        ctx.Genres.RemoveRange(ctx.Genres);
        await ctx.SaveChangesAsync();

        var genre = new Genre
        {
            Createdat = DateTime.UtcNow,
            Id = "1",
            Name = "thriller"
        };
        ctx.Genres.Add(genre);
        await ctx.SaveChangesAsync();

        for (int i = 2; i < 11; i++)
        {
                    var author = new Author
                    {
                        Createdat = DateTime.UtcNow,
                        Id = i+"",
                        Name = "Bob"
                    };
                    ctx.Authors.Add(author);
        }
        
        var a = new Author
        {
            Createdat = DateTime.UtcNow,
            Id = "1",
            Name = "Bob"
        };
        ctx.Authors.Add(a);
     
        var book = new Book
        {
            Createdat = DateTime.UtcNow,
            Id = "1",
            Pages = 42,
            Title = "Bobs book",
            Genre = genre,
            Authors = new List<Author>()
            {
                
                a
            }
        };
        ctx.Books.Add(book);

        await ctx.SaveChangesAsync();

    }
}