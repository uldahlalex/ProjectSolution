using SieveQueryBuilder;

/// <summary>
/// Query model for Author with configured filterable and sortable properties
/// </summary>
public class AuthorQueryModel : ISieveQueryModel
{
    public int BooksCount { get; set; }

}