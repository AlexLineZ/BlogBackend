using System.Collections;
using System.ComponentModel.DataAnnotations;
using BlogBackend.Models.DTO;

namespace BlogBackend.Models;

public class Community
{
    [Key] 
    public Guid Id { get; set; }
    
    [Required]
    public DateTime CreateTime { get; set; }
    
    [Required]
    [MinLength(1)]
    public String Name { get; set; }

    public String Description { get; set; }
    
    [Required]
    public Boolean IsClosed { get; set; }

    [Required]
    public Int32 SubscribersCount { get; set; }
    
    [Required]
    public List<CommunityUser> CommunityUsers { get; set; }
    
    public List<Post> Posts { get; set; }
    
    public Community() {}

    public Community(Guid id, DateTime createTime, String name, String description, Boolean isClosed,
        Int32 subscribersCount, List<CommunityUser> communityUsers, List<Post> posts)
    {
        Id = id;
        CreateTime = createTime;
        Name = name;
        Description = description;
        IsClosed = isClosed;
        SubscribersCount = subscribersCount;
        CommunityUsers = communityUsers;
        Posts = posts;
    }
}