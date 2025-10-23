using System.Text.Json;
using System.Text.Json.Serialization;
using api;
using api.Services;
using dataccess;
using Sieve.Models;
using SieveQueryBuilder;

namespace tests;

public class SieveQuerying(ILibraryService libraryService, 
    MyDbContext ctx,
    ITestOutputHelper outputHelper,
    ISeeder seeder)
{
    // Basic Filtering Tests
    [Fact]
    public async Task FilterAuthors_ByExactName()
    {
        await seeder.Seed();

        var randomAuthor = ctx.Authors.OrderBy(a => Guid.NewGuid()).First();

        var builder = SieveQueryBuilder<Author>.Create()
            .FilterEquals(a => a.Name, randomAuthor.Name);
        
        outputHelper.WriteLine("Requesting with model: "+JsonSerializer.Serialize(builder.BuildSieveModel()));
        
        var actual = await libraryService.GetAuthors(builder.BuildSieveModel());
        
        outputHelper.WriteLine("JSON serialized actual result: "+JsonSerializer.Serialize(actual, new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 1024,
            WriteIndented = true
        }));
        
        Assert.Contains(actual, a => a == randomAuthor);

    }

    [Fact]
    public async Task FilterAuthors_ByNameContains()
    {
        
    }

    [Fact]
    public async Task FilterAuthors_ByNameStartsWith() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByExactTitle() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByTitleContains() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByPageCountEquals() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByPageCountGreaterThan() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByPageCountLessThan() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByPageCountRange() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByGenreId() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterGenres_ByExactName() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterGenres_ByNameContains() { throw new NotImplementedException(); }

    // Date Filtering Tests
    [Fact]
    public async Task FilterAuthors_ByCreatedAfterDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAuthors_ByCreatedBeforeDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByCreatedAfterDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByCreatedBeforeDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByCreatedDateRange() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterGenres_ByCreatedAfterDate() { throw new NotImplementedException(); }

    // Combined Filtering Tests
    [Fact]
    public async Task FilterBooks_ByTitleAndPageCount() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByTitleAndGenre() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_ByPageCountAndDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAuthors_ByNameAndDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_MultipleConditionsWithOr() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_MultipleConditionsWithAnd() { throw new NotImplementedException(); }

    // Sorting Tests - Ascending
    [Fact]
    public async Task SortAuthors_ByNameAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortAuthors_ByCreatedDateAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByTitleAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByPageCountAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByCreatedDateAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortGenres_ByNameAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortGenres_ByCreatedDateAscending() { throw new NotImplementedException(); }

    // Sorting Tests - Descending
    [Fact]
    public async Task SortAuthors_ByNameDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortAuthors_ByCreatedDateDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByTitleDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByPageCountDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByCreatedDateDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortGenres_ByNameDescending() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortGenres_ByCreatedDateDescending() { throw new NotImplementedException(); }

    // Multi-level Sorting Tests
    [Fact]
    public async Task SortBooks_ByTitleThenPageCount() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByGenreThenTitle() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortAuthors_ByNameThenCreatedDate() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_ByPageCountDescendingThenTitleAscending() { throw new NotImplementedException(); }

    // Pagination Tests
    [Fact]
    public async Task PaginateAuthors_FirstPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateAuthors_SecondPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateAuthors_LastPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateBooks_PageSize10() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateBooks_PageSize25() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateBooks_PageSize50() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateGenres_PageSize5() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateBooks_NavigateThroughAllPages() { throw new NotImplementedException(); }

    // Combined Filter + Sort Tests
    [Fact]
    public async Task FilterAndSortAuthors_ByNameContainsSortedAscending() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAndSortBooks_ByPageRangeSortedByTitle() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAndSortBooks_ByGenreSortedByPageCount() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAndSortBooks_ByTitleContainsSortedByDate() { throw new NotImplementedException(); }

    // Combined Filter + Pagination Tests
    [Fact]
    public async Task FilterAndPaginateAuthors_ByNameFirstPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAndPaginateBooks_ByPageRangeWithPagination() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterAndPaginateBooks_ByGenreWithPagination() { throw new NotImplementedException(); }

    // Combined Sort + Pagination Tests
    [Fact]
    public async Task SortAndPaginateAuthors_ByNameAscendingFirstPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortAndPaginateBooks_ByPageCountDescendingSecondPage() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortAndPaginateGenres_ByNameAscendingWithPageSize5() { throw new NotImplementedException(); }

    // Combined Filter + Sort + Pagination Tests
    [Fact]
    public async Task FilterSortAndPaginateAuthors_CompleteQuery() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterSortAndPaginateBooks_ByPageRangeSortedPaginated() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterSortAndPaginateBooks_ByGenreSortedByTitlePaginated() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterSortAndPaginateBooks_MultipleFiltersSortedPaginated() { throw new NotImplementedException(); }

    // Edge Cases
    [Fact]
    public async Task FilterAuthors_NoMatchesReturnsEmpty() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_InvalidFilterReturnsError() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateAuthors_PageBeyondRangeReturnsEmpty() { throw new NotImplementedException(); }

    [Fact]
    public async Task PaginateBooks_PageSize0ReturnsError() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_InvalidPropertyReturnsError() { throw new NotImplementedException(); }

    // Case Sensitivity Tests
    [Fact]
    public async Task FilterAuthors_CaseInsensitiveNameSearch() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_CaseInsensitiveTitleSearch() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterGenres_CaseInsensitiveNameSearch() { throw new NotImplementedException(); }

    // Null/Empty Value Tests
    [Fact]
    public async Task FilterBooks_WithNullGenreId() { throw new NotImplementedException(); }

    [Fact]
    public async Task FilterBooks_WithEmptyFilter() { throw new NotImplementedException(); }

    [Fact]
    public async Task SortBooks_WithNullValues() { throw new NotImplementedException(); }
}