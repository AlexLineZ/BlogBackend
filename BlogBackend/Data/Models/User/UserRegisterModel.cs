using System.ComponentModel.DataAnnotations;
using BlogBackend.Data.Models.User;

namespace BlogBackend.Models;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }
    
    [Required]
    [MinLength(6)]
    public String Password { get; set; }
    
    [Required]
    [EmailAddress]
    public String Email { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [RegularExpression("^\\+\\d{1,3} \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$")]
    public String? PhoneNumber { get; set; }
}