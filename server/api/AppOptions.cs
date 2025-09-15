using System.ComponentModel.DataAnnotations;

public class AppOptions
{
    [Required][MinLength(1)]
    public string Db { get; set; }
}