using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api")]
[ApiController]
public class AuthorController: ControllerBase
{
    [HttpGet]
    [Route("author/list")]
    public async Task<IActionResult> GetPostInformation()
    {
        return Ok(new { test = "test" });
    }
}