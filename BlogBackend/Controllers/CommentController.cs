using BlogBackend.Models.Comments;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _commentService.CreateComment(comment, id, userId);
        return Ok();
    }

    [HttpPut]
    [Route("comment/{id}")]
    public async Task<IActionResult> EditComment(Guid id, [FromBody] UpdateCommentDto comment)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _commentService.UpdateComment(id, comment, userId);
        return Ok();
    }

    [HttpDelete]
    [Route("comment/{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _commentService.DeleteComment(id, userId);
        return Ok();
    }
}