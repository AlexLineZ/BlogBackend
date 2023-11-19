using BlogBackend.Data.Models.User;
using BlogBackend.Models;
using BlogBackend.Models.Info;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Controllers;

[Route("api/account")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new MessageResponse
            {
                Status = "Error",
                Message = "User model is invalid"
            });
        }

        try
        {
            return await _userService.Register(model);
        }
        
        catch (InvalidOperationException e)
        {
            return BadRequest(new MessageResponse
            {
                Status = "Error",
                Message = e.Message
            });
        }
        
        catch (Exception e)
        {
            return StatusCode(500, new MessageResponse
            {
                Status = "Error",
                Message = "Something went wrong"
            });
        }
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new MessageResponse
            {
                Status = "Error",
                Message = "User model is invalid"
            });
        }

        try
        {
            return await _userService.Login(model);
        }
        
        catch (InvalidOperationException e)
        {
            return StatusCode(400, new MessageResponse
            {
                Status = "Error",
                Message = "Invalid login or password"
            });
        }
        
        catch (Exception e)
        {
            return StatusCode(500, new MessageResponse
            {
                Status = "Error",
                Message = e.Message
            });
        }
        
    }
}