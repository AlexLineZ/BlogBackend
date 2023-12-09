using BlogBackend.Data.Models.User;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
            return BadRequest(ModelState);
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
            return BadRequest(ModelState);
        }
        
        var response = await _userService.Login(model);
        return Ok(new { token = response.Token });
    }

    [HttpPost]
    [Route("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(token))
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        
        await _userService.Logout(token);
        return Ok();
    }

    [HttpGet]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        var response = _userService.GetProfile(userId);
        return Ok(response);
    }
    
    [HttpPut]
    [Route("profile")]
    [Authorize]
    public async Task<IActionResult> PutProfile(UserEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var tokenUserId = User.Claims.FirstOrDefault(claim => claim.Type == "id")?.Value;
        var userId = tokenUserId == null? Guid.Empty : Guid.Parse(tokenUserId);
        
        await _userService.PutProfile(model, userId);
        return Ok();
    }
}