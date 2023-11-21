using System.Security.Authentication;
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
            return BadRequest();
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
            return BadRequest();
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
            return BadRequest();
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
            return Unauthorized();
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
    
    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> PutProfile(UserEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }


        try
        {
            var token = Request.Headers["Authorization"].ToString().Substring(7);
            return await _userService.PutProfile(model, token);
        }
        catch (InvalidDataException)
        {
            return Unauthorized();
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