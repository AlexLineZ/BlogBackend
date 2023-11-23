using BlogBackend.Models;
using BlogBackend.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDTO>> GetCommunity();
    Task<List<CommunityUserDTO>> GetUserCommunity(String token);
    Task<CommunityFullDTO> GetCommunityById(Guid communityId);
    Task<CommunityRole> GetUserRole(Guid communityId, String token);
    Task<IActionResult> Subscribe(Guid communityId, String token);
    Task<IActionResult> Unsubscribe(Guid communityId, String token);
}