using BlogBackend.Data.Models.User;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Interfaces;

public interface IUserService
{
    Task<TokenResponse> Register([FromBody] UserRegisterModel model);
    Task<TokenResponse> Login([FromBody] LoginCredentials model);
    Task Logout(String token);
    UserDto GetProfile(String token);
    Task PutProfile(UserEditModel model, String token);
}