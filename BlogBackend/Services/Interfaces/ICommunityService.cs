using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDto>> GetCommunity();
    Task<List<CommunityUserDto>> GetUserCommunity(String token);
    Task<CommunityFullDto> GetCommunityById(Guid communityId);
    Task<PostGroup> GetCommunityPost(Guid communityId, List<Guid>? tags, 
        PostSorting? sorting, Int32 page, Int32 size);
    Task CreatePost(Guid communityId, CreatePostDto post, String token);
    Task<CommunityRole> GetUserRole(Guid communityId, String token);
    Task<IActionResult> Subscribe(Guid communityId, String token);
    Task<IActionResult> Unsubscribe(Guid communityId, String token);
}