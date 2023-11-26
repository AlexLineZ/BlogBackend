using BlogBackend.Models;
using BlogBackend.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDto>> GetCommunity();
    Task<List<CommunityUserDto>> GetUserCommunity(String token);
    Task<CommunityFullDto> GetCommunityById(Guid communityId);
    //Task<List<PostFullDto>> GetCommunityPost(Guid communityId);
    Task CreatePost(Guid communityId, CreatePostDto post, String token);
    Task<CommunityRole> GetUserRole(Guid communityId, String token);
    Task<IActionResult> Subscribe(Guid communityId, String token);
    Task<IActionResult> Unsubscribe(Guid communityId, String token);
}