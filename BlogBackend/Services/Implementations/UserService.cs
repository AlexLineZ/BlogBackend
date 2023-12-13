using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using BlogBackend.Data;
using BlogBackend.Data.Models.User;
using BlogBackend.Exceptions;
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
            Guid.NewGuid(),
            model.FullName,
            model.BirthDate,
            model.Gender,
            model.Email,
            model.PhoneNumber,
            GenerateSha256(model.Password),
            new List<Guid>(),
            new List<Post>(),
            new List<Guid>()
            );
        
        var token = _tokenService.GenerateJwtToken(_configuration, user);
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return new TokenResponse(token);
    }

    public async Task<TokenResponse> Login([FromBody] LoginCredentials model)
    {
        var user = _dbContext.Users
            .FirstOrDefault(x =>
            x.Email == model.Email && x.Password == GenerateSha256(model.Password));

        if (user == null)
        {
            throw new InvalidCredentialException("Incorrect login or password");
        }

        var token = _tokenService.GenerateJwtToken(_configuration, user);

        return new TokenResponse(token);
    }

    public async Task Logout(String token)
    {
        var findToken = _dbContext.ExpiredTokens.FirstOrDefault(x =>
            x.Token == token);
        
        if (findToken != null)
        {
            throw new UnauthorizedAccessException("User already logged out");
        }
        
        _dbContext.ExpiredTokens.Add(new ExpiredTokenStorage{Token = token});
        await _dbContext.SaveChangesAsync();
    }

    public UserDto GetProfile(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        
        var user = _dbContext.Users.FirstOrDefault(x =>
            userId == x.Id);

        if (user == null)
        {
            throw new ResourceNotFoundException("User not found");
        }
        
        var userDtO = new UserDto(user.Id, user.CreateTime, user.FullName,
            user.BirthDate, user.Gender, user.Email, user.PhoneNumber);

        return userDtO;
    }

    public async Task PutProfile(UserEditModel model, Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }
        
        var user = _dbContext.Users.FirstOrDefault(x =>
            userId == x.Id);

        if (user == null)
        {
            throw new ResourceNotFoundException("User not found");
        }

        var checkEmail = _dbContext.Users.Any(x => x.Email == model.Email && x.Id != userId);

        if (checkEmail)
        {
            throw new InvalidCredentialException("This email is already use by another user");
        }

        var checkPhoneNumber = _dbContext.Users.Any(x => x.PhoneNumber == model.PhoneNumber && x.Id != userId);
        
        if (checkPhoneNumber)
        {
            throw new InvalidCredentialException("This phoneNumber is already use by another user");
        }
        
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.BirthDate = model.BirthDate;
        user.Gender = model.Gender;
        user.PhoneNumber = model.PhoneNumber;
        
        await _dbContext.SaveChangesAsync();
    }
    
    private string GenerateSha256(string input)
    {
        using SHA256 hash = SHA256.Create();
        return Convert.ToHexString(hash.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}