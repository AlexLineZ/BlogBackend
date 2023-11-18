using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models;

public class LoginCredentials
{
    [Required]
    [EmailAddress]
    public String Email { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Password { get; set; }
}