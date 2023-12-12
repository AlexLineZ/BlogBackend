using System.ComponentModel.DataAnnotations;
using BlogBackend.Validation;

namespace BlogBackend.Models.DTO;

public class CreatePostDto
{
    [Key]
    [MinLength(5)]
    public String Title { get; set; }

    [Required]
    [MinLength(5)]
    public String Description { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "ReadingTime must be positive")]
    public Int32 ReadingTime { get; set; }
    
    [Url]
    public String Image { get; set; }
    
    [ValidAddressId]
    public Guid? AddressId { get; set; }

    [Required]
    [MinLength(1)]
    public List<Guid> Tags { get; set; }
}