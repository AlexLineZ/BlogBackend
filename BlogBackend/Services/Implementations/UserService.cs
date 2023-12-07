using System.Security.Authentication;
using BlogBackend.Data;
using BlogBackend.Data.Models.User;
using BlogBackend.Exceptions;
using BlogBackend.Helpers;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogBackend.Services.Implementations;

public class UserService: IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    
    public UserService(AppDbContext dbContext, IConfiguration configuration, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> Register([FromBody] UserRegisterModel model)
    {
        var isUserRegistered = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email);

        if (isUserRegistered != null)
        {
            throw new InvalidCredentialException($"Email {model.Email} already registered");
        }
        
        var user = new User(
            new Guid(),
            model.FullName,
            model.BirthDate,
            model.Gender,
            model.Email,
            model.PhoneNumber,
            UserHelper.GenerateSHA256(model.Password),
            new List<Guid>(),
            new List<Post>(),
            new List<Guid>()
            );
        
        var token = _configuration.GenerateJwtToken(user);
        
        await _dbContext.Users.AddAsync(user);
        await _tokenService.AddOrEditToken(token, user);
        await _dbContext.SaveChangesAsync();

        return new TokenResponse(token);
    }

    public async Task<TokenResponse> Login([FromBody] LoginCredentials model)
    {
        var user = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email && x.Password == UserHelper.GenerateSHA256(model.Password));

        if (user == null)
        {
            throw new InvalidCredentialException("Incorrect login or password");
        }

        var token = _configuration.GenerateJwtToken(user);
        await _tokenService.AddOrEditToken(token, user);
        
        return new TokenResponse(token);
    }

    public async Task Logout(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            x.Token == token);
        if (findToken == null)
        {
            throw new UnauthorizedAccessException("Token not found");
        }
        _dbContext.Tokens.Remove(findToken);
        await _dbContext.SaveChangesAsync();
    }

    public UserDto GetProfile(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);
        
        if (findToken == null)
        {
            throw new UnauthorizedAccessException("Token is not found");
        }
        
        if (_tokenService.IsTokenFresh(findToken) == false)
        {
            throw new UnauthorizedAccessException("Token expired");
        }

        var user = _dbContext.Users.FirstOrDefault(x =>
            findToken.UserId == x.Id);

        if (user == null)
        {
            throw new ResourceNotFoundException("User not found");
        }
        
        var userDtO = new UserDto(user.Id, user.CreateTime, user.FullName,
            user.BirthDate, user.Gender, user.Email, user.PhoneNumber);

        return userDtO;
    }

    public async Task PutProfile(UserEditModel model, String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);

        if (findToken == null)
        {
            throw new UnauthorizedAccessException("Token is not found");
        }
        
        if (_tokenService.IsTokenFresh(findToken) == false)
        {
            throw new UnauthorizedAccessException("Token expired");
        }
        
        var user = _dbContext.Users.FirstOrDefault(x =>
            findToken.UserId == x.Id);

        if (user == null)
        {
            throw new ResourceNotFoundException("User not found");
        }
        
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.BirthDate = model.BirthDate;
        user.Gender = model.Gender;
        user.PhoneNumber = model.PhoneNumber;
        
        await _dbContext.SaveChangesAsync();
    }
}