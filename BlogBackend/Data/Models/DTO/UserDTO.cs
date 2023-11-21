using System.ComponentModel.DataAnnotations;
using BlogBackend.Data.Models.User;

namespace BlogBackend.Models.DTO;

public class UserDTO
{
    [Key]
    public Guid Id { get; set; }
    public DateTime CreateTime { get; set; }
    [Required]
    public String FullName { get; set; }
    public DateTime? BirthDate { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Required]
    public String Email { get; set; }
    public String PhoneNumber { get; set; }
    
    public UserDTO() {}
    public UserDTO(Guid id, DateTime createTime, string fullName, 
        DateTime? birthDate, Gender gender, string email, string phoneNumber)
    {
        Id = id;
        CreateTime = createTime;
        FullName = fullName;
        BirthDate = birthDate;
        Gender = gender;
        Email = email;
        PhoneNumber = phoneNumber;
    }
}