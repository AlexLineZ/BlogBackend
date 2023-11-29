using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models;

public class Tag
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; }
}