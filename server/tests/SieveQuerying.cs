// using System.Text.Json;
// using api;
// using api.Services;
// using dataccess;
// using Sieve.Models;
//
// namespace tests;
//
// public class SieveQuerying(ILibraryService libraryService, ITestOutputHelper outputHelper, ISeeder seeder)
// {
//
//     [Fact]
//     public async Task MyTest()
//     {
//         // Seed the database with Bob_0 to Bob_9
//         await seeder.Seed();
//
//         // First, let's verify data exists without filters
//         var allAuthors = await libraryService.GetAuthors(new SieveModel());
//         outputHelper.WriteLine($"Total authors in DB: {allAuthors.Count}");
//         foreach (var author in allAuthors)
//         {
//             outputHelper.WriteLine($"  - {author.Name}");
//         }
//
//         var model = SieveQueryBuilder<Author>.Create()
//             .FilterContains(a => a.Name, "Bob_0")
//             .SortBy(a => a.Createdat)
//             .BuildSieveModel();
//         outputHelper.WriteLine($"Sieve Model: {JsonSerializer.Serialize(model)}");
//
//         var res = await libraryService.GetAuthors(model);
//         outputHelper.WriteLine($"Filtered results count: {res.Count}");
//         foreach (var author in res)
//         {
//             outputHelper.WriteLine($"  - {author.Name}");
//         }
//
//         Assert.Single(res);
//         Assert.Equal("Bob_0", res.First().Name);
//     }
// }