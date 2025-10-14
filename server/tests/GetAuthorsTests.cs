using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using api;
using api.Services;
using dataccess;

namespace tests;

public class GetAuthorsTests(MyDbContext ctx,
    ITestOutputHelper outputHelper, 
    ILibraryService libraryService,
    ISeeder seeder)
{
    //Happy path
    [Fact]
    public async Task GetAuthors_CanExtractAnAuthor()
    {
        //Arrange
        await seeder.Seed();
        
        
        //Act
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.Name,
            Skip = 0,
            Take = 1
        });
        
        //Assert
        Assert.Single(actual);
    }
    
    //Happy path
    [Fact]
    public async Task GetAuthors_CanSortAuthorsByNameAndPaginate()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();
        
        
        //Act
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.Name,
            Skip = 0,
            Take = 2
        });
        
        outputHelper.WriteLine(JsonSerializer.Serialize(actual, 
            new JsonSerializerOptions() 
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
             WriteIndented = true
        }));
        
        //Assert
        
        Assert.Equal(2, actual.Count);
        Assert.Equal("Bob_0", actual.First().Name);
        Assert.Equal("Bob_1", actual[1].Name);
    }
    
    //Happy path
    [Fact]
    public async Task GetAuthors_CanSortAuthorsByNameDecendingAndPaginate()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();
        
        
        //Act
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.NameDescending,
            Skip = 0,
            Take = 2
        });
        
        outputHelper.WriteLine(JsonSerializer.Serialize(actual, 
            new JsonSerializerOptions() 
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            }));
        
        //Assert
        
        Assert.Equal(2, actual.Count);
        Assert.Equal("Bob_9", actual.First().Name);
        Assert.Equal("Bob_8", actual[1].Name);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(0, Int32.MaxValue)]
    [InlineData(10_000, 0)]
    public async Task GetAuthors_ThrowsException_WhenDataAnnotationsAreViolated(int skip, int take)
    {
        var dto = new GetAuthorsRequestDto()
        {
            Skip = skip,
            Take = take,
            Ordering = AuthorOrderingOptions.Name
        };
        await Assert.ThrowsAnyAsync<ValidationException>(async () 
            => await libraryService.GetAuthors(dto));
    }
    
    
    [Fact]
    public async Task GetAuthors_CanSortAuthorsByNumberOfBooksWritten()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        var book = new Book()
        {
            Createdat = DateTime.UtcNow,
            Id = "-1",
            Title = "Test book",
            Pages = 42
        };
        ctx.Books.Add(book);
        ctx.SaveChanges();

        var author = ctx.Authors.First(a => a.Id.Equals("7"));
        author.Books.Clear();
        var books = ctx.Books;
         foreach (var b in books)
        {
            author.Books.Add(b);
        }

        ctx.SaveChanges();


        //Act
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            Ordering = AuthorOrderingOptions.NumberOfBooksPublished,
            Skip = 0,
            Take = 3
        });

        outputHelper.WriteLine(JsonSerializer.Serialize(actual,
            new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            }));

        //Assert
        Assert.Equal("Bob_7", actual.First().Name);
        Assert.Equal("Bob_0", actual[1].Name);
        Assert.Equal("Bob_1", actual[2].Name);
    }

    [Fact]
    public async Task GetAuthors_CanFilterByNameContains_GenericSystem()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        //Act - Using new generic filter system
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            SortBy = "Name",
            SortAscending = true,
            Skip = 0,
            Take = 10,
            Filters = new List<FilterDto>()
            {
                new FilterDto()
                {
                    PropertyName = "Name",
                    Operator = FilterOperator.Contains,
                    Value = "_5"
                }
            }
        });

        //Assert
        Assert.Single(actual);
        Assert.Equal("Bob_5", actual.First().Name);
    }

    [Fact]
    public async Task GetAuthors_CanFilterByBooksCount_GenericSystem()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        //Act - Get authors with at least 2 books using generic filter
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            SortBy = "Name",
            Skip = 0,
            Take = 10,
            Filters = new List<FilterDto>()
            {
                new FilterDto()
                {
                    PropertyName = "Books.Count",
                    Operator = FilterOperator.GreaterThanOrEqual,
                    Value = "2"
                }
            }
        });

        //Assert
        Assert.All(actual, author => Assert.True(author.Books.Count >= 2));
    }

    [Fact]
    public async Task GetAuthors_CanSortByName_GenericSystem()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        //Act - Using new generic sorting system
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            SortBy = "Name",
            SortAscending = true,
            Skip = 0,
            Take = 3
        });

        //Assert
        Assert.Equal(3, actual.Count);
        Assert.Equal("Bob_0", actual[0].Name);
        Assert.Equal("Bob_1", actual[1].Name);
        Assert.Equal("Bob_2", actual[2].Name);
    }

    [Fact]
    public async Task GetAuthors_CanSortByBooksCount_GenericSystem()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        //Act - Sort by Books.Count descending
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            SortBy = "Books.Count",
            SortAscending = false,
            Skip = 0,
            Take = 3
        });

        outputHelper.WriteLine(JsonSerializer.Serialize(actual,
            new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            }));

        //Assert
        Assert.Equal(3, actual.Count);
        // Authors should be sorted by book count descending
        Assert.True(actual[0].Books.Count >= actual[1].Books.Count);
        Assert.True(actual[1].Books.Count >= actual[2].Books.Count);
    }

    [Fact]
    public async Task GetAuthors_CanCombineMultipleFilters_GenericSystem()
    {
        //Arrange
        await new SeederWithRelations(ctx).Seed();

        //Act - Filter by name contains "Bob" AND Books.Count >= 1
        var actual = await libraryService.GetAuthors(new GetAuthorsRequestDto()
        {
            SortBy = "Name",
            Skip = 0,
            Take = 10,
            Filters = new List<FilterDto>()
            {
                new FilterDto()
                {
                    PropertyName = "Name",
                    Operator = FilterOperator.Contains,
                    Value = "Bob"
                },
                new FilterDto()
                {
                    PropertyName = "Books.Count",
                    Operator = FilterOperator.GreaterThanOrEqual,
                    Value = "1"
                }
            }
        });

        //Assert
        Assert.All(actual, author =>
        {
            Assert.Contains("Bob", author.Name);
            Assert.True(author.Books.Count >= 1);
        });
    }
}