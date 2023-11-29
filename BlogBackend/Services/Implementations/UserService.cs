﻿using System.Security.Authentication;
using BlogBackend.Data;
using BlogBackend.Data.Models.User;
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

    public async Task<TokenResponse> Register([FromBody] UserRegisterModel model) //автомапперы
    {
        var isUserRegistered = _dbContext.Users.FirstOrDefault(x =>
            x.Email == model.Email);

        if (isUserRegistered != null)
        {
            throw new InvalidCredentialException("This Email already registered");
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
            new List<Guid>(),
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

    public async Task<IActionResult> Logout(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            x.Token == token);
        if (findToken == null)
        {
            throw new InvalidOperationException("Token not found");
        }
        _dbContext.Tokens.Remove(findToken);
        await _dbContext.SaveChangesAsync();

        return new OkResult();
    }

    public UserDto GetProfile(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);
        
        if (findToken != null)
        {
            if (_tokenService.IsTokenFresh(findToken) == false)
            {
                throw new InvalidOperationException("Token expired");
            }
        }

        var user = _dbContext.Users.FirstOrDefault(x =>
            findToken.UserId == x.Id);

        if (user == null)
        {
            throw new InvalidOperationException("Not authorized");
        }
        
        var userDTO = new UserDto(user.Id, user.CreateTime, user.FullName,
            user.BirthDate, user.Gender, user.Email, user.PhoneNumber);

        return userDTO;
    }

    public async Task<IActionResult> PutProfile(UserEditModel model, String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);
        
        if (findToken != null)
        {
            if (_tokenService.IsTokenFresh(findToken) == false)
            {
                throw new InvalidOperationException("Token expired");
            }
        }
        
        var user = _dbContext.Users.FirstOrDefault(x =>
            findToken.UserId == x.Id);

        if (user == null)
        {
            throw new InvalidOperationException("Not authorized");
        }
        else
        {
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
        }
        await _dbContext.SaveChangesAsync();
        return new OkResult();
    }
}