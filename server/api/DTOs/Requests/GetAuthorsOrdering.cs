using AutoFilterer.Types;
using dataccess;

public enum GetAuthorsOrdering
{
    Name,
    CreatedAt,
    NumberOfBooks
}

public record GetAuthorsRequestDto
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public GetAuthorsOrdering Ordering { get; set; }
    public GetAuthorsFiltering Filtering { get; set; }
}

public class GetAuthorsFiltering : FilterBase
{
    
}
