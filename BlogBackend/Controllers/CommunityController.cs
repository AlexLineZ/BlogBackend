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
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        var communityUserList = await _communityService.GetUserCommunity(token);
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
        var token = await HttpContext.GetTokenAsync("access_token");
        var postGroup = await _communityService.GetCommunityPost(id, tags, sorting, page, size, token);
        return Ok(postGroup);
    }

    [HttpPost]
    [Route("{id}/post")]
    public async Task<IActionResult> CreatePost(Guid id, CreatePostDto post)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        var postId = await _communityService.CreatePost(id, post, token);
        return Ok(postId);
    } 
    
    [HttpGet]
    [Route("{id}/role")]
    public async Task<IActionResult> GetUserRole(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        var userRole = await _communityService.GetUserRole(id, token);
        return Ok(userRole);
    }
    
    [HttpPost]
    [Route("{id}/subscribe")]
    public async Task<IActionResult> SubscribeCommunity(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        await _communityService.Subscribe(id, token);
        return Ok();
    }
    
    [HttpDelete]
    [Route("{id}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeCommunity(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        await _communityService.Unsubscribe(id, token);
        return Ok();
    }
}