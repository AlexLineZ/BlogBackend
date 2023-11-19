using BlogBackend.Data.Models.User;
using BlogBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface IUserService
{
    Task<IActionResult> Register([FromBody] UserRegisterModel model);
    
    TokenResponse Login([FromBody] LoginCredentials model);
}