using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using api;
using api.Services;
using dataccess;

namespace tests;

public class GetAuthorTests(MyDbContext ctx, ITestOutputHelper outputHelper, ILibraryService libraryService)
{
   
   //Happy path = pagination works with skip 0 take 1 + name alphabetical ordering
   [Fact]
   public async Task GetAuthorsWithPaginationAndNameOrdering_Works()
   {
      //Arrange
      var seeder = new SeederWithRelations(ctx);
      await seeder.Seed();
      var dto = new GetAuthorsRequestDto()
      {
         Skip = 0,
         Take = 1,
         Ordering = AuthorOrderingOptions.Name
      };
      
      //Act
      var actual = await libraryService.GetAuthors(dto);
      
      //Assert
      Assert.Equal("0", actual.First().Id);

   }
   
   //Unhappy path = Pagination parameters are violated
   [Theory]
   [InlineData(-1, 1)]
   [InlineData(1, -1)]
   [InlineData(0, 1001)]
   [InlineData(5, 0)]
   public async Task GetAuthorsWithViolatedPaginationProperties_Fail(int skip, int take)
   {
      var dto = new GetAuthorsRequestDto()
      {
         Skip = skip,
         Take = take,
         Ordering = AuthorOrderingOptions.Name
      };
      
      //Act + Assert
      await Assert.ThrowsAnyAsync<Exception>(async () => await libraryService.GetAuthors(dto));
   }
   
   
   //Happy path = pagination works with skip 0 take 1 + name alphabetical ordering
   [Fact]
   public async Task GetAuthorsWithPaginationAndBooksWrittenOrdering_Works()
   {
      //Arrange
      var seeder = new SeederWithRelations(ctx);
      await seeder.Seed();
      
      //Add additional data for arrange phase
      var book = new Book()
      {
         Id = "-1",
         Createdat = DateTime.UtcNow,
         Pages = 42,
         Title = "New test book"
      };
      ctx.Books.Add(book);
      ctx.SaveChanges();

      var authorToAddNewBookTo = ctx.Authors.First(a => a.Id.Equals("0"));
      authorToAddNewBookTo.Books.Clear();
      await foreach (var ctxBook in ctx.Books)
      {
         authorToAddNewBookTo.Books.Add(ctxBook);
      }

      var authorToHave1Book = ctx.Authors.First(a => a.Id.Equals("7"));
      authorToHave1Book.Books.Add(book);

      ctx.SaveChanges();
      
      
      var dto = new GetAuthorsRequestDto()
      {
         Skip = 0,
         Take = 3,
         Ordering = AuthorOrderingOptions.NumberOfBooksPublished
      };
      
      //Act
      var actual = await libraryService.GetAuthors(dto);
      
      outputHelper.WriteLine(JsonSerializer.Serialize(actual, new JsonSerializerOptions()
      {
         WriteIndented = true,
         ReferenceHandler = ReferenceHandler.IgnoreCycles
      }));
      
      //Assert
      Assert.Equal("0", actual.First().Id);
      Assert.Equal("7", actual[1].Id);
      Assert.Equal("1", actual[2].Id);
   }
}