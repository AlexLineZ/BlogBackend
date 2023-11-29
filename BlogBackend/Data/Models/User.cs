using System.ComponentModel.DataAnnotations;
using BlogBackend.Data.Models.User;

namespace BlogBackend.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; } 
    
    [Required]
    [EmailAddress]
    public String Email { get; set; }
    
    [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$")]
    public String PhoneNumber { get; set; }
    
    [Required]
    [MinLength(6)]
    public String Password { get; set; }
    
    public List<Guid>? Communities { get; set; }
    
    public List<Guid>? Posts { get; set; }
    
    public List<Guid>? Likes { get; set; }

    public User() { }
    public User(Guid id, string fullName, DateTime birthDate, 
        Gender gender, string email, string phoneNumber, string password, List<Guid>? communities, List<Guid>? posts, List<Guid>? likes)
    {
        Id = id;
        CreateTime = DateTime.UtcNow;
        FullName = fullName; 
        BirthDate = birthDate;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Communities = communities;
        Posts = posts;
        Likes = likes;
    }
}