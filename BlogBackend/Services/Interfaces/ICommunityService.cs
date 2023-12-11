using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityDto>> GetCommunity();
    Task<List<CommunityUserDto>> GetUserCommunity(Guid userId);
    Task<CommunityFullDto> GetCommunityById(Guid communityId);
    Task<PostGroup> GetCommunityPost(Guid communityId, List<Guid>? tags, 
        PostSorting? sorting, Int32 page, Int32 size, Guid userId);
    Task<Guid> CreatePost(Guid communityId, CreatePostDto post, Guid userId);
    Task<CommunityRole?> GetUserRole(Guid communityId, Guid userId);
    Task Subscribe(Guid communityId, Guid userId);
    Task Unsubscribe(Guid communityId, Guid userId);
}