using api.Etc.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace api.Etc.Controllers;

public class LibraryController(ILibraryService libraryService)
{
    [HttpGet(nameof(GetAuthors))]
    public List<AuthorDto> GetAuthors()
    {
        throw new NotImplementedException();
    }   
    
    [HttpGet(nameof(GetBooks))]
    public List<BookDto> GetBooks()
    {
        throw new NotImplementedException();
    }   
    
    [HttpGet(nameof(GetGenres))]
    public List<GenreDto> GetGenres()
    {
        throw new NotImplementedException();
    }   
}