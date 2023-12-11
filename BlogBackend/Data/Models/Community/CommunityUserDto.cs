using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class CommunityUserDto
{
    [Key] 
    public Guid UserId { get; set; }
    [Required]
    public Guid CommunityId { get; set; }
    [Required]
    public CommunityRole Role { get; set; }
    
    public CommunityUserDto() {}

    public CommunityUserDto(Guid userId, Guid communityId, CommunityRole role)
    {
        UserId = userId;
        CommunityId = communityId;
        Role = role;
    }
}