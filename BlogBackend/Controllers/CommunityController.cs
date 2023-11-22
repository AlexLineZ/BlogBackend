using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
        return Ok(new { test = "test" });
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetCommunityInformation(Guid id)
    {
        return Ok(new { test = "test" });
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
        return Ok(new { test = "test" });
    }
    
    [HttpPost]
    [Route("{id}/subscribe")]
    public async Task<IActionResult> SubscribeCommunity(Guid id)
    {
        return Ok(new { test = "test" });
    }
    
    [HttpDelete]
    [Route("{id}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeCommunity(Guid id)
    {
        return Ok(new { test = "test" });
    }
}