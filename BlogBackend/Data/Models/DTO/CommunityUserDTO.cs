using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class CommunityUserDTO
{
    [Key] 
    public Guid UserId { get; set; }
    [Required]
    public Guid CommunityId { get; set; }
    [Required]
    public CommunityRole Role { get; set; }
    
    public CommunityUserDTO() {}

    public CommunityUserDTO(Guid userId, Guid communityId, CommunityRole role)
    {
        UserId = userId;
        CommunityId = communityId;
        Role = role;
    }
}