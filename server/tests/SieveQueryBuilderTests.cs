using api.Services;
using dataccess;

namespace tests;

/// <summary>
/// Demonstrates type-safe Sieve query building
/// </summary>
public class SieveQueryBuilderTests
{
    private readonly ITestOutputHelper _outputHelper;

    public SieveQueryBuilderTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void BuildFilterEquals_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterEquals(a => a.Name, "Bob_5")
            .PageSize(10)
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name==Bob_5", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildFilterContains_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "_5")
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name@=_5", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildMultipleFilters_CombinesCorrectly()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterNotEquals(a => a.Name, "Bob_0")
            .BuildFiltersString();

        // Assert
        Assert.Equal("Name@=Bob,Name!=Bob_0", query);
        _outputHelper.WriteLine($"Filter: {query}");
    }

    [Fact]
    public void BuildSortByAscending_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortBy(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildSortByDescending_CreatesCorrectQueryString()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortByDescending(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("-Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildMultipleSorts_CombinesCorrectly()
    {
        // Arrange & Act
        var query = SieveQueryBuilder<Author>.Create()
            .SortByDescending(a => a.Createdat)
            .SortBy(a => a.Name)
            .BuildSortsString();

        // Assert
        Assert.Equal("-Createdat,Name", query);
        _outputHelper.WriteLine($"Sort: {query}");
    }

    [Fact]
    public void BuildSieveModel_CreatesCompleteModel()
    {
        // Arrange & Act
        var sieveModel = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .SortBy(a => a.Name)
            .Page(2)
            .PageSize(10)
            .BuildSieveModel();

        // Assert
        Assert.Equal("Name@=Bob", sieveModel.Filters);
        Assert.Equal("Name", sieveModel.Sorts);
        Assert.Equal(2, sieveModel.Page);
        Assert.Equal(10, sieveModel.PageSize);

        _outputHelper.WriteLine($"Filters: {sieveModel.Filters}");
        _outputHelper.WriteLine($"Sorts: {sieveModel.Sorts}");
        _outputHelper.WriteLine($"Page: {sieveModel.Page}");
        _outputHelper.WriteLine($"PageSize: {sieveModel.PageSize}");
    }

    [Fact]
    public void BuildQueryString_CreatesCompleteHttpQueryString()
    {
        // Arrange & Act
        var queryString = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterNotEquals(a => a.Name, "Bob_0")
            .SortBy(a => a.Name)
            .Page(2)
            .PageSize(10)
            .BuildQueryString();

        // Assert
        Assert.Contains("filters=Name%40%3DBob%2CName%21%3DBob_0", queryString);
        Assert.Contains("sorts=Name", queryString);
        Assert.Contains("page=2", queryString);
        Assert.Contains("pageSize=10", queryString);

        _outputHelper.WriteLine($"Query String: {queryString}");
    }

    [Fact]
    public void BuildWithCustomPropertyName_ForMappedProperties()
    {
        // Arrange & Act - BooksCount is mapped in ApplicationSieveProcessor
        var query = SieveQueryBuilder<Author>.Create()
            .FilterByName("BooksCount", ">=", 5)
            .SortByName("BooksCount", descending: true)
            .BuildFiltersString();

        var sort = SieveQueryBuilder<Author>.Create()
            .SortByName("BooksCount", descending: true)
            .BuildSortsString();

        // Assert
        Assert.Equal("BooksCount>=5", query);
        Assert.Equal("-BooksCount", sort);

        _outputHelper.WriteLine($"Filter: {query}");
        _outputHelper.WriteLine($"Sort: {sort}");
    }

    [Fact]
    public void BuildComparisonFilters_AllOperators()
    {
        // Arrange & Act
        var builder = SieveQueryBuilder<Book>.Create();

        var greaterThan = builder.FilterGreaterThan(b => b.Pages, 200).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var lessThan = builder.FilterLessThan(b => b.Pages, 500).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var greaterOrEqual = builder.FilterGreaterThanOrEqual(b => b.Pages, 200).BuildFiltersString();

        builder = SieveQueryBuilder<Book>.Create();
        var lessOrEqual = builder.FilterLessThanOrEqual(b => b.Pages, 500).BuildFiltersString();

        // Assert
        Assert.Equal("Pages>200", greaterThan);
        Assert.Equal("Pages<500", lessThan);
        Assert.Equal("Pages>=200", greaterOrEqual);
        Assert.Equal("Pages<=500", lessOrEqual);

        _outputHelper.WriteLine($"Greater than: {greaterThan}");
        _outputHelper.WriteLine($"Less than: {lessThan}");
        _outputHelper.WriteLine($"Greater or equal: {greaterOrEqual}");
        _outputHelper.WriteLine($"Less or equal: {lessOrEqual}");
    }

    [Fact]
    public void BuildDateTimeFilter_WorksCorrectly()
    {
        // Arrange
        var targetDate = new DateTime(2024, 1, 1);

        // Act
        var query = SieveQueryBuilder<Author>.Create()
            .FilterGreaterThanOrEqual(a => a.Createdat, targetDate)
            .BuildFiltersString();

        // Assert
        Assert.Contains("Createdat>=", query);
        _outputHelper.WriteLine($"DateTime Filter: {query}");
    }

    [Fact]
    public void BuildComplexQuery_RealWorldExample()
    {
        // Arrange & Act - Simulate a complex search scenario
        var sieveModel = SieveQueryBuilder<Author>.Create()
            .FilterContains(a => a.Name, "Bob")
            .FilterGreaterThanOrEqual(a => a.Createdat, DateTime.Now.AddDays(-30))
            .FilterByName("BooksCount", ">=", 3)
            .SortByDescending(a => a.Createdat)
            .SortBy(a => a.Name)
            .Page(1)
            .PageSize(20)
            .BuildSieveModel();

        // Assert
        Assert.Contains("Name@=Bob", sieveModel.Filters);
        Assert.Contains("Createdat>=", sieveModel.Filters);
        Assert.Contains("BooksCount>=3", sieveModel.Filters);
        Assert.Equal("-Createdat,Name", sieveModel.Sorts);
        Assert.Equal(1, sieveModel.Page);
        Assert.Equal(20, sieveModel.PageSize);

        _outputHelper.WriteLine($"Complex Query:");
        _outputHelper.WriteLine($"  Filters: {sieveModel.Filters}");
        _outputHelper.WriteLine($"  Sorts: {sieveModel.Sorts}");
        _outputHelper.WriteLine($"  Page: {sieveModel.Page}");
        _outputHelper.WriteLine($"  PageSize: {sieveModel.PageSize}");
    }
}
