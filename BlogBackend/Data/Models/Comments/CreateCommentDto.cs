using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.Comments;

public class CreateCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public String Content { get; set; }
    
    public Guid? ParentId { get; set; }
}