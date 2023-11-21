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
        return Ok(new { token = "test" });
    }
}