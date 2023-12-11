using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api")]
[ApiController]
public class AuthorController: ControllerBase
{
    private readonly IAuthorService _authorService;
    
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    
    [HttpGet]
    [Route("author/list")]
    public async Task<IActionResult> GetPostInformation()
    {
        var authors = await _authorService.GetAuthorList();
        return Ok(authors);
    }
}