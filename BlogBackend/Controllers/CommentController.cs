using BlogBackend.Models.Comments;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;


[ApiController]
[Route("api")]
public class CommentController: ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    [Route("comment/{id}/tree")]
    public async Task<IActionResult> GetCommentTree(Guid id)
    {
        var comment = await _commentService.GetCommentsTree(id);
        return Ok(comment);
    }
    
    [HttpPost]
    [Route("post/{id}/comment")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentDto comment)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        await _commentService.CreateComment(comment, id, token);
        return Ok();
    }

    [HttpPut]
    [Route("comment/{id}")]
    public async Task<IActionResult> EditComment(Guid id, [FromBody] UpdateCommentDto comment)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        await _commentService.UpdateComment(id, comment, token);
        return Ok();
    }

    [HttpDelete]
    [Route("comment/{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        await _commentService.DeleteComment(id, token);
        return Ok();
    }
}