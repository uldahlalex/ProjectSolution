using dataccess;

namespace api;

public class SieveTestSeeder(MyDbContext ctx) : ISeeder
{
    public async Task Seed()
    {
        await ctx.Database.EnsureCreatedAsync();
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
                Createdat = DateTime.UtcNow.AddDays(-(i * 18)) // Deterministic dates based on index
            });
        }
        ctx.Genres.AddRange(genres);
        await ctx.SaveChangesAsync();

        // Create authors (100 authors) - deterministic, idempotent
        var authors = new List<Author>();
        var fullNames = new[]
        {
            "John Smith", "Jane Johnson", "Michael Williams", "Sarah Brown", "David Jones",
            "Emma Garcia", "Robert Miller", "Lisa Davis", "William Rodriguez", "Mary Martinez",
            "James Hernandez", "Patricia Lopez", "Richard Gonzalez", "Jennifer Wilson", "Thomas Anderson",
            "Linda Thomas", "Charles Taylor", "Barbara Moore", "Daniel Jackson", "Elizabeth Martin",
            "Matthew Lee", "Susan Thompson", "Anthony White", "Jessica Harris", "Mark Clark",
            "Ashley Lewis", "Donald Robinson", "Dorothy Walker", "Steven Hall", "Nancy Allen",
            "Paul Young", "Karen King", "George Wright", "Betty Scott", "Edward Green",
            "Sandra Adams", "Brian Baker", "Donna Nelson", "Ronald Carter", "Carol Mitchell",
            "Kevin Perez", "Michelle Roberts", "Jason Turner", "Kimberly Phillips", "Gary Campbell",
            "Lisa Parker", "Timothy Evans", "Helen Edwards", "Jeffrey Collins", "Deborah Stewart",
            "Ryan Sanchez", "Sarah Morris", "Jacob Rogers", "Margaret Reed", "Nicholas Cook",
            "Emily Morgan", "Eric Bell", "Stephanie Murphy", "Jonathan Bailey", "Amanda Rivera",
            "Stephen Cooper", "Melissa Richardson", "Larry Cox", "Debra Howard", "Justin Ward",
            "Rebecca Torres", "Scott Peterson", "Sharon Gray", "Brandon Ramirez", "Cynthia James",
            "Raymond Watson", "Kathleen Brooks", "Samuel Kelly", "Amy Sanders", "Gregory Price",
            "Angela Bennett", "Alexander Wood", "Shirley Barnes", "Patrick Ross", "Brenda Henderson",
            "Frank Coleman", "Pamela Jenkins", "Benjamin Perry", "Anna Powell", "Jack Long",
            "Nicole Patterson", "Dennis Hughes", "Catherine Flores", "Jerry Washington", "Heather Butler",
            "Tyler Simmons", "Diane Foster", "Aaron Gonzales", "Ruth Bryant", "Jose Alexander",
            "Virginia Russell", "Adam Griffin", "Christina Diaz", "Henry Hayes", "Janet Myers"
        };

        for (int i = 0; i < 100; i++)
        {
            authors.Add(new Author
            {
                Id = Guid.NewGuid().ToString(),
                Name = fullNames[i],
                Createdat = DateTime.UtcNow.AddDays(-((i * 10) % 1000)) // Deterministic dates based on index
            });
        }
        ctx.Authors.AddRange(authors);
        await ctx.SaveChangesAsync();

        // Create books (500 books with varied data) - deterministic, idempotent
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
            var prefix = titlePrefixes[i % titlePrefixes.Length];
            var middle = titleMiddles[(i / titlePrefixes.Length) % titleMiddles.Length];
            var suffix = titleSuffixes[(i / (titlePrefixes.Length * titleMiddles.Length)) % titleSuffixes.Length];

            var book = new Book
            {
                Id = Guid.NewGuid().ToString(),
                Title = $"{prefix} {middle} {suffix}",
                Pages = 50 + ((i * 19) % 950), // Deterministic page count between 50-1000
                Createdat = DateTime.UtcNow.AddDays(-(i * 4)), // Deterministic dates based on index
                Genreid = genres[i % genres.Count].Id // Deterministic genre assignment
            };

            books.Add(book);
        }
        ctx.Books.AddRange(books);
        await ctx.SaveChangesAsync();

        // Create author-book relationships (many-to-many) - deterministic, idempotent
        // Each book will have 1-3 authors based on deterministic logic
        for (int i = 0; i < books.Count; i++)
        {
            var book = books[i];
            var numAuthors = 1 + (i % 3); // Deterministic: cycles through 1, 2, 3 authors

            // Deterministically select authors based on book index
            for (int j = 0; j < numAuthors; j++)
            {
                var authorIndex = (i * 7 + j * 13) % authors.Count; // Deterministic author selection
                book.Authors.Add(authors[authorIndex]);
            }
        }
        await ctx.SaveChangesAsync();
    }
}
