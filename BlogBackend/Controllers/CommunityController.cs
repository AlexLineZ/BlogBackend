using System.Security.Claims;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace BlogBackend.Controllers;

[Route("api/community")]
[ApiController]
public class CommunityController: ControllerBase
{
    private readonly ICommunityService _communityService;

    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommunityList()
    {
        var communityList = await _communityService.GetCommunity();
        return Ok(communityList);
    }

    [HttpGet]
    [Route("my")]
    public async Task<IActionResult> GetUserCommunityList()
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var communityUserList = await _communityService.GetUserCommunity(userId);
        return Ok(communityUserList);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetCommunityInformation(Guid id)
    {
        var communityInformation = await _communityService.GetCommunityById(id);
        return Ok(communityInformation);
    }

    [HttpGet]
    [Route("{id}/post")]
    public async Task<IActionResult> GetCommunityPosts(Guid id,
        [FromQuery] List<Guid>? tags,
        [FromQuery] PostSorting? sorting,
        [FromQuery] Int32 page = 1,
        [FromQuery] Int32 size = 5
    )
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var postGroup = await _communityService.GetCommunityPost(id, tags, sorting, page, size, userId);
        return Ok(postGroup);
    }

    [HttpPost]
    [Authorize]
    [Route("{id}/post")]
    public async Task<IActionResult> CreatePost(Guid id, CreatePostDto post)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var postId = await _communityService.CreatePost(id, post, userId);
        return Ok(postId);
    } 
    
    [HttpGet]
    [Route("{id}/role")]
    public async Task<IActionResult> GetUserRole(Guid id)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var userRole = await _communityService.GetUserRole(id, userId);
        return Ok(userRole);
    }
    
    [HttpPost]
    [Route("{id}/subscribe")]
    public async Task<IActionResult> SubscribeCommunity(Guid id)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _communityService.Subscribe(id, userId);
        return Ok();
    }
    
    [HttpDelete]
    [Route("{id}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeCommunity(Guid id)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _communityService.Unsubscribe(id, userId);
        return Ok();
    }
}