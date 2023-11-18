using BlogBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface IUserService
{
    IActionResult Register([FromBody] UserRegisterModel model);
    
    String Login([FromBody] LoginCredentials model);
}