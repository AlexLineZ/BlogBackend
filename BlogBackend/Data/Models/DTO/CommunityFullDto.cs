using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class CommunityFullDto
{
    [Key] 
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    public String Name { get; set; }

    public String Description { get; set; }
    
    [Required]
    public Boolean IsClosed { get; set; }

    [Required]
    public Int32 SubscribersCount { get; set; }
    
    [Required]
    public List<UserDto> Administrators { get; set; }
    
    public CommunityFullDto() {}

    public CommunityFullDto(Guid id, DateTime createTime, String name, String description, Boolean isClosed,
        Int32 subscribersCount, List<UserDto> administrators)
    {
        Id = id;
        CreateTime = createTime;
        Name = name;
        Description = description;
        IsClosed = isClosed;
        SubscribersCount = subscribersCount;
        Administrators = administrators;
    }
}