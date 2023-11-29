using System.ComponentModel.DataAnnotations;
using BlogBackend.Data.Models.User;

namespace BlogBackend.Models.DTO;

public class AuthorDto
{
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }
    public DateTime? Birthday { get; set; }
    [Required]
    public Gender Gender { get; set; }
    public Int32 Posts { get; set; }
    public Int32 Likes { get; set; }
    public DateTime Created { get; set; }
}