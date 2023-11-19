using BlogBackend.Data;
using BlogBackend.Helpers;
using BlogBackend.Models;
using BlogBackend.Data.Models.User;
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

    public async Task<IActionResult> Register([FromBody] UserRegisterModel model)
    {
        var isUserRegistered = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email);

        if (isUserRegistered != null)
        {
            throw new InvalidOperationException("This Email already registered");
        }
        
        var user = new User(
            new Guid(),
            model.FullName,
            model.BirthDate,
            model.Gender,
            model.Email,
            model.PhoneNumber,
            UserHelper.GenerateSHA256(model.Password)
        );
        
        var token = _configuration.GenerateJwtToken(user);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return new OkObjectResult(new { Message = "Registration successful." });
    }

    public TokenResponse Login([FromBody] LoginCredentials model)
    {
        var user = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email && x.Password == model.Password);
        
        if (user == null) return null;

        var tokenResponse = new TokenResponse(_configuration.GenerateJwtToken(user));

        return tokenResponse;
    }
}