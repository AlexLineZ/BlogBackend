using System.ComponentModel.DataAnnotations;
using BlogBackend.Models.Comments;
using BlogBackend.Models.DTO;

namespace BlogBackend.Models;

public class Post
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
    public Int32 ReadingTime { get; set; }
    
    [Url]
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
    public Int32 CommentsCount { get; set; }
    
    public List<Guid>? Tags { get; set; }
    
    public List<Comment> Comments { get; set; }
}