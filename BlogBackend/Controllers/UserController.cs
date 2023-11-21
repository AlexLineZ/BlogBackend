using System.Security.Authentication;
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
            var response = await _userService.Register(model);
            return Ok(new
            {
                token = response.Token
            });
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
            var response = await _userService.Login(model);
            return Ok(new
            {
                token = response.Token
            });

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

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Substring(7);
            return await _userService.Logout(token);
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

    [HttpGet]
    [Route("profile")]
    public IActionResult GetProfile()
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Substring(7);
            return Ok(_userService.GetProfile(token));
        }
        catch (InvalidCredentialException)
        {
            return StatusCode(401);
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