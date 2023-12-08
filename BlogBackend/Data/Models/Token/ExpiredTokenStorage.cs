using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models;

public class ExpiredTokenStorage
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public String Token { get; set; }
}