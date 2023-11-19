using BlogBackend.Data.Models.User;
using BlogBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface IUserService
{
    Task<TokenResponse> Register([FromBody] UserRegisterModel model);
    Task<TokenResponse> Login([FromBody] LoginCredentials model);
}