using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.Comments;

public class Comment
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Content { get; set; }
    
    public DateTime? ModifiedDate { get; set; }
    
    public DateTime? DeleteDate { get; set; }
    
    [Required]
    public Guid AuthorId { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Author { get; set; }
    
    [Required]
    public Int32 SubComments { get; set; }
    
    public List<Comment> SubCommentsList { get; set; }
    
    public Guid? ParentId { get; set; }
}