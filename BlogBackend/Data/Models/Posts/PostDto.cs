using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class PostDto
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Title { get; set; }
    
    [Required]
    public String Description { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "ReadingTime must be positive")]
    public Int32 ReadingTime { get; set; }
    
    public String Image { get; set; }
    
    [Required]
    public Guid AuthorId { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Author { get; set; }
    
    public Guid? CommunityId { get; set; }
    
    public String? CommunityName { get; set; }
    
    public Guid? AddressId { get; set; }
    
    [Required]
    public Int32 Likes { get; set; }
    
    [Required]
    public Boolean HasLike { get; set; }
    
    [Required]
    public Int32 CommentsCount { get; set; }
    
    public List<TagDto>? Tags { get; set; }
}