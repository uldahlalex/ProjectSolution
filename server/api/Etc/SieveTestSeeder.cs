using dataccess;

namespace api;

public class SieveTestSeeder(MyDbContext ctx) : ISeeder
{
    public async Task Seed()
    {
        // Clear existing data
        ctx.Books.RemoveRange(ctx.Books);
        ctx.Authors.RemoveRange(ctx.Authors);
        ctx.Genres.RemoveRange(ctx.Genres);
        await ctx.SaveChangesAsync();

        // Create genres (20 genres)
        var genres = new List<Genre>();
        var genreNames = new[]
        {
            "Science Fiction", "Fantasy", "Mystery", "Thriller", "Romance",
            "Horror", "Historical Fiction", "Biography", "Self-Help", "Business",
            "Philosophy", "Poetry", "Drama", "Adventure", "Crime",
            "Western", "Dystopian", "Paranormal", "Contemporary", "Classic"
        };

        for (int i = 0; i < genreNames.Length; i++)
        {
            genres.Add(new Genre
            {
                Id = Guid.NewGuid().ToString(),
                Name = genreNames[i],
                Createdat = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365))
            });
        }
        ctx.Genres.AddRange(genres);
        await ctx.SaveChangesAsync();

        // Create authors (100 authors)
        var authors = new List<Author>();
        var firstNames = new[]
        {
            "John", "Jane", "Michael", "Sarah", "David", "Emma", "Robert", "Lisa",
            "William", "Mary", "James", "Patricia", "Richard", "Jennifer", "Thomas",
            "Linda", "Charles", "Barbara", "Daniel", "Elizabeth", "Matthew", "Susan",
            "Anthony", "Jessica", "Mark", "Ashley", "Donald", "Dorothy", "Steven", "Nancy"
        };
        var lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
            "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
            "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Thompson", "White",
            "Harris", "Clark", "Lewis", "Robinson", "Walker", "Hall", "Allen"
        };

        for (int i = 0; i < 100; i++)
        {
            var firstName = firstNames[Random.Shared.Next(firstNames.Length)];
            var lastName = lastNames[Random.Shared.Next(lastNames.Length)];

            authors.Add(new Author
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"{firstName} {lastName}",
                Createdat = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 1000))
            });
        }
        ctx.Authors.AddRange(authors);
        await ctx.SaveChangesAsync();

        // Create books (500 books with varied data)
        var books = new List<Book>();
        var titlePrefixes = new[]
        {
            "The", "A", "An", "Tales of", "Chronicles of", "Legend of",
            "Secrets of", "Adventures in", "Journey to", "Mystery of"
        };
        var titleMiddles = new[]
        {
            "Dark", "Lost", "Hidden", "Ancient", "Forgotten", "Eternal",
            "Silent", "Broken", "Golden", "Silver", "Crimson", "Emerald"
        };
        var titleSuffixes = new[]
        {
            "Kingdom", "Empire", "City", "Forest", "Mountain", "Ocean",
            "Desert", "Island", "Castle", "Tower", "Shadow", "Light",
            "Dream", "Night", "Dawn", "Storm", "Fire", "Ice"
        };

        for (int i = 0; i < 500; i++)
        {
            var prefix = titlePrefixes[Random.Shared.Next(titlePrefixes.Length)];
            var middle = titleMiddles[Random.Shared.Next(titleMiddles.Length)];
            var suffix = titleSuffixes[Random.Shared.Next(titleSuffixes.Length)];

            var book = new Book
            {
                Id = Guid.NewGuid().ToString(),
                Title = $"{prefix} {middle} {suffix}",
                Pages = Random.Shared.Next(50, 1000),
                Createdat = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 2000)),
                Genreid = genres[Random.Shared.Next(genres.Count)].Id
            };

            books.Add(book);
        }
        ctx.Books.AddRange(books);
        await ctx.SaveChangesAsync();

        // Create author-book relationships (many-to-many)
        // Each book will have 1-3 authors
        foreach (var book in books)
        {
            var numAuthors = Random.Shared.Next(1, 4);
            var selectedAuthors = authors
                .OrderBy(_ => Random.Shared.Next())
                .Take(numAuthors)
                .ToList();

            foreach (var author in selectedAuthors)
            {
                book.Authors.Add(author);
            }
        }
        await ctx.SaveChangesAsync();
    }
}
