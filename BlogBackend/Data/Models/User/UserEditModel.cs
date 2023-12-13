using System.ComponentModel.DataAnnotations;
using BlogBackend.Validation;

namespace BlogBackend.Data.Models.User;

public class UserEditModel
{
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }

    [Required]
    [RegularExpression("[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\\.[a-zA-Z0-9_-]+",
        ErrorMessage = "Invalid email address")]
    public String Email { get; set; }
    
    [BirthDateValidation]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
        ErrorMessage = "PhoneNumber is not valid")]
    public String PhoneNumber { get; set; }
}