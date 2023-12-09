using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api")]
[ApiController]
public class PostController: ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    [Route("post")]
    public async Task<IActionResult> GetPostList(
        [FromQuery] List<Guid>? tags,
        [FromQuery] String? author,
        [FromQuery] Int32? min,
        [FromQuery] Int32? max,
        [FromQuery] PostSorting? sorting,
        [FromQuery] Boolean onlyMyCommunities = false,
        [FromQuery] Int32 page = 1,
        [FromQuery] Int32 size = 5
        )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        Guid? userId = tokenUserId == null? null : Guid.Parse(tokenUserId);

        var pageList = await  _postService.GetPostList(tags, author, min, max,
            sorting, onlyMyCommunities, page, size, userId);
        return Ok(pageList);
    }

    [HttpPost]
    [Route("post")]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var id = await _postService.CreatePost(model, userId);
        return Ok(id);
    }
    
    [HttpGet]
    [Route("post/{id}")]
    public async Task<IActionResult> GetPostInformation(Guid id)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        Guid? userId = tokenUserId == null? null : Guid.Parse(tokenUserId);
        
        var postFull = await _postService.GetPost(id, userId);
        return Ok(postFull);
    }
    
    [HttpPost]
    [Route("post/{postId}/like")]
    [Authorize]
    public async Task<IActionResult> LikePost(Guid postId)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _postService.LikePost(postId, userId);
        return Ok();
    }
    
    [HttpDelete]
    [Route("post/{postId}/like")]
    [Authorize]
    public async Task<IActionResult> DislikePost(Guid postId)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _postService.DislikePost(postId, userId);
        return Ok();
    }
}

