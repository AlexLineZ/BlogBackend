using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var token = Request.Headers["Authorization"].ToString().Substring(7);
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
    public async Task<IActionResult> GetCommunityPosts(Guid id)
    {
        return Ok(new { test = "test" });
    }

    [HttpPost]
    [Route("{id}/post")]
    public async Task<IActionResult> CreatePost(Guid id)
    {
        return Ok(new { test = "test" });
    }
    
    [HttpGet]
    [Route("{id}/role")]
    public async Task<IActionResult> GetUserRole(Guid id)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        var userRole = await _communityService.GetUserRole(id, token);
        return Ok(userRole.GetDisplayName());
    }
    
    [HttpPost]
    [Route("{id}/subscribe")]
    public async Task<IActionResult> SubscribeCommunity(Guid id)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        await _communityService.Subscribe(id, token);
        return Ok();
    }
    
    [HttpDelete]
    [Route("{id}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeCommunity(Guid id)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        await _communityService.Unsubscribe(id, token);
        return Ok();
    }
}