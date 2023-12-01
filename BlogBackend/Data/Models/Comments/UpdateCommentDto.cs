using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.Comments;

public class UpdateCommentDto
{
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public String Content { get; set; }
}