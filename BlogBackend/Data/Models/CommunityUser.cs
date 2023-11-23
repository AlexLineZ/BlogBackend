using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models;

public class CommunityUser
{
    [Key] 
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid CommunityId { get; set; }
    [Required]
    public CommunityRole Role { get; set; }
    
    public CommunityUser() {}

    public CommunityUser(Guid id, Guid userId, Guid communityId, CommunityRole role)
    {
        Id = id;
        UserId = userId;
        CommunityId = communityId;
        Role = role;
    }
}