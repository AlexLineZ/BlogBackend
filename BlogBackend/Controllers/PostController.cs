using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api")]
[ApiController]
public class PostController: ControllerBase
{
    [HttpGet]
    [Route("post")]
    public async Task<IActionResult> GetPostList(
        [FromQuery] List<String> tags,
        [FromQuery] String author,
        [FromQuery] Int32? min,
        [FromQuery] Int32? max,
        [FromQuery] String sorting,
        [FromQuery] Boolean onlyMyCommunities = false,
        [FromQuery] Int32 page = 1,
        [FromQuery] Int32 size = 5
        )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        return Ok(new { test = "test" });
    }

    [HttpPost]
    [Route("post")]
    public async Task<IActionResult> CreatePost()
    {
        return Ok(new { test = "test" });
    }
    
    [HttpGet]
    [Route("post/{id}")]
    public async Task<IActionResult> GetPostInformation(Guid id)
    {
        return Ok(new { test = "test" });
    }
    
    [HttpPost]
    [Route("post/{postId}/like")]
    public async Task<IActionResult> LikePost(Guid postId)
    {
        return Ok(new { test = "test" });
    }
    
    [HttpDelete]
    [Route("post/{postId}/like")]
    public async Task<IActionResult> DislikePost(Guid postId)
    {
        return Ok(new { test = "test" });
    }
}

