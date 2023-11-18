using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Implementations;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult Register([FromBody] UserRegisterModel model)
    {
        return new OkObjectResult(new { Message = "Registration successful." });
    }
}