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

        return new OkObjectResult(new
        {
            token = token
        });
    }

    public async Task<IActionResult> Login([FromBody] LoginCredentials model)
    {
        var user = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email && x.Password == UserHelper.GenerateSHA256(model.Password));

        if (user == null)
        {
            throw new InvalidOperationException("This user is not registered");
        }

        var token = _configuration.GenerateJwtToken(user);

        return new OkObjectResult(new
        {
            token = token
        });
    }
}