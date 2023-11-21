using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Data.Models.User;

public class UserEditModel
{
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }

    [Required]
    [EmailAddress]
    public String Email { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [RegularExpression("^\\+\\d{1,3} \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$")]
    public String PhoneNumber { get; set; }
}