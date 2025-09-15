using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace api.Etc.Controllers;

public record CreateBookRequestDto
{
    [Required][Range(1,int.MaxValue)] public int Pages { get; set; }
    [Required] [MinLength(1)] public string Title { get; set; }
}