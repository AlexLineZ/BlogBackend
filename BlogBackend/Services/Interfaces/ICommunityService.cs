using BlogBackend.Models.DTO;

namespace BlogBackend.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDTO>> GetCommunity();

    Task<List<CommunityUserDTO>> GetUserCommunity(String token);

}