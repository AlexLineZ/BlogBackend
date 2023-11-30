using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;


[ApiController]
[Route("api")]
public class CommentController: ControllerBase
{
    [HttpGet]
    [Route("comment/{id}/tree")]
    public async Task<IActionResult> GetCommentTree(int id)
    {
        return Ok(new {tree = "test"});
    }
    
    [HttpPost]
    [Route("post/{id}/comment")]
    public async Task<IActionResult> AddComment(int id)
    {
        return Ok(new {tree = "test"});
    }

    [HttpPut]
    [Route("comment/{id}")]
    public async Task<IActionResult> EditComment(int id)
    {
        return Ok(new {tree = "test"});
    }

    [HttpDelete]
    [Route("comment/{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        return Ok(new {tree = "test"});
    }
}