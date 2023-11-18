using BlogBackend.Data;
using BlogBackend.Helpers;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Implementations;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    
    public UserService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public IActionResult Register([FromBody] UserRegisterModel model)
    {
        return new OkObjectResult(new { Message = "Registration successful." });
    }

    public String Login([FromBody] LoginCredentials model)
    {
        var user = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email && x.Password == model.Password);
        
        if (user == null) return null;

        var token = _configuration.GenerateJwtToken(user);

        return token;
    }
}