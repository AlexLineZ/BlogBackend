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
        
        catch (InvalidOperationException ex)
        {
            return BadRequest(new MessageResponse
            {
                Status = "Error",
                Message = ex.Message
            });
        }
        
        catch (Exception ex)
        {
            return StatusCode(500, new MessageResponse
            {
                Status = "Error",
                Message = "Something went wrong"
            });
        }
    }
/*
    public IActionResult Login([FromBody] LoginCredentials model)
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
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    */
}