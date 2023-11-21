
using BlogBackend.Data.Models.User;
using BlogBackend.Models;
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

        var response = await _userService.Register(model);
        return Ok(new { token = response.Token });
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginCredentials model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var response = await _userService.Login(model);
        return Ok(new { token = response.Token });
    }

    [HttpPost]
    [Route("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        return await _userService.Logout(token);
    }

    [HttpGet]
    [Route("profile")]
    public IActionResult GetProfile()
    {
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        return Ok(_userService.GetProfile(token));
    }
    
    [HttpPut]
    [Route("profile")]
    public async Task<IActionResult> PutProfile(UserEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var token = Request.Headers["Authorization"].ToString().Substring(7);
        return await _userService.PutProfile(model, token);
    }
}