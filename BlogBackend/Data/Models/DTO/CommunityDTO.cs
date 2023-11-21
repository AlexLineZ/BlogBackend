using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class CommunityDTO
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
    
    public CommunityDTO() {}

    public CommunityDTO(Guid id, DateTime createTime, String name, String description, Boolean isClosed,
        Int32 subscribersCount)
    {
        Id = id;
        CreateTime = createTime;
        Name = name;
        Description = description;
        IsClosed = isClosed;
        SubscribersCount = subscribersCount;
    }
}