using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
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
        
        var token = await HttpContext.GetTokenAsync("access_token");

        var pageList = await  _postService.GetPostList(tags, author, min, max,
            sorting, onlyMyCommunities, page, size, token);
        return Ok(pageList);
    }

    [HttpPost]
    [Route("post")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        var id = await _postService.CreatePost(model, token);
        return Ok(id);
    }
    
    [HttpGet]
    [Route("post/{id}")]
    public async Task<IActionResult> GetPostInformation(Guid id)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var postFull = await _postService.GetPost(id, token);
        return Ok(postFull);
    }
    
    [HttpPost]
    [Route("post/{postId}/like")]
    public async Task<IActionResult> LikePost(Guid postId)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        await _postService.LikePost(postId, token);
        return Ok();
    }
    
    [HttpDelete]
    [Route("post/{postId}/like")]
    public async Task<IActionResult> DislikePost(Guid postId)
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException();
        }
        await _postService.DislikePost(postId, token);
        return Ok();
    }
}

