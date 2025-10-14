// using System.ComponentModel.DataAnnotations;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using api;
// using api.Services;
// using dataccess;
//
// namespace tests;
//
// public class GetAuthorsSieveTests(MyDbContext ctx,
//     ITestOutputHelper outputHelper,
//     ILibraryService libraryService,
//     ISeeder seeder)
// {
//     [Fact]
//     public async Task GetAuthorsSieve_CanFilterByNameContains()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Filter using Sieve syntax: Name@=_5 means "Name contains _5"
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Filters = "Name@=_5",
//             PageSize = 10
//         });
//
//         //Assert
//         Assert.Single(actual);
//         Assert.Equal("Bob_5", actual.First().Name);
//     }
//
//     // Note: Books.Count filtering doesn't work with Sieve + EF Core many-to-many
//     // This is a known limitation - collection counts can't be translated to SQL in filters
//     // For collection count filtering, use the custom generic filter system instead
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanSortByNameAscending()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Sort by Name ascending (default)
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Sorts = "Name",
//             PageSize = 3
//         });
//
//         //Assert
//         Assert.Equal(3, actual.Count);
//         Assert.Equal("Bob_0", actual[0].Name);
//         Assert.Equal("Bob_1", actual[1].Name);
//         Assert.Equal("Bob_2", actual[2].Name);
//     }
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanSortByNameDescending()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Sort by Name descending (- prefix)
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Sorts = "-Name",
//             PageSize = 3
//         });
//
//         outputHelper.WriteLine(JsonSerializer.Serialize(actual,
//             new JsonSerializerOptions()
//             {
//                 ReferenceHandler = ReferenceHandler.IgnoreCycles,
//                 WriteIndented = true
//             }));
//
//         //Assert
//         Assert.Equal(3, actual.Count);
//         Assert.Equal("Bob_9", actual[0].Name);
//         Assert.Equal("Bob_8", actual[1].Name);
//         Assert.Equal("Bob_7", actual[2].Name);
//     }
//
//     // Note: Books.Count sorting also has limitations with Sieve + EF Core
//     // Collection counts can't reliably be sorted in complex many-to-many scenarios
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanCombineMultipleFilters()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Multiple filters with comma: Name@=Bob,Name!=Bob_0 (exclude Bob_0)
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Filters = "Name@=Bob,Name!=Bob_0",
//             Sorts = "Name",
//             PageSize = 10
//         });
//
//         //Assert
//         Assert.All(actual, author =>
//         {
//             Assert.Contains("Bob", author.Name);
//             Assert.NotEqual("Bob_0", author.Name);
//         });
//     }
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanUseEqualsOperator()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Equals operator: Name==Bob_3
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Filters = "Name==Bob_3",
//             PageSize = 10
//         });
//
//         //Assert
//         Assert.Single(actual);
//         Assert.Equal("Bob_3", actual.First().Name);
//     }
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanUsePagination()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Page 2 with PageSize 3
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Sorts = "Name",
//             Page = 2,
//             PageSize = 3
//         });
//
//         //Assert
//         Assert.Equal(3, actual.Count);
//         Assert.Equal("Bob_3", actual[0].Name);
//         Assert.Equal("Bob_4", actual[1].Name);
//         Assert.Equal("Bob_5", actual[2].Name);
//     }
//
//     [Fact]
//     public async Task GetAuthorsSieve_CanSortByMultipleFields()
//     {
//         //Arrange
//         await new SeederWithRelations(ctx).Seed();
//
//         //Act - Sort by Createdat descending, then Name ascending
//         var actual = await libraryService.GetAuthorsSieve(new GetAuthorsSieveRequestDto()
//         {
//             Sorts = "-Createdat,Name",
//             PageSize = 5
//         });
//
//         outputHelper.WriteLine(JsonSerializer.Serialize(actual,
//             new JsonSerializerOptions()
//             {
//                 ReferenceHandler = ReferenceHandler.IgnoreCycles,
//                 WriteIndented = true
//             }));
//
//         //Assert - Should be sorted properly
//         Assert.Equal(5, actual.Count);
//         for (int i = 0; i < actual.Count - 1; i++)
//         {
//             if (actual[i].Createdat == actual[i + 1].Createdat)
//             {
//                 // If dates are equal, names should be in ascending order
//                 Assert.True(string.Compare(actual[i].Name, actual[i + 1].Name) <= 0);
//             }
//         }
//     }
// }
