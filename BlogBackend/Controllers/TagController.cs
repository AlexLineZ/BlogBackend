using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api")]
[ApiController]
public class TagController: ControllerBase
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }
    
    [HttpGet("tag")]
    public async Task<IActionResult> GetTagsList()
    {
        var list = await _tagService.GetTagList();
        return Ok(list);
    }
}