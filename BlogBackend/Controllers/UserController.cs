using BlogBackend.Models;
using BlogBackend.Services.Implementations;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] UserRegisterModel model)
    {
        return Ok();
    }
    
}