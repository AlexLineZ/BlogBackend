namespace BlogBackend.Models;

public class UserRegisterModel
{
    public String FullName { get; set; }
    
    public String Password { get; set; }
    
    public String Email { get; set; }
    
    public DateTime BirthDate { get; set; }
    
    public Gender Gender { get; set; }
    
    public String PhoneNumber { get; set; }
}