using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogBackend.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly AppDbContext _dbContext;
    
    public TokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public String GenerateJwtToken(IConfiguration configuration, User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public async Task AddOrEditToken(String token, User user)
    {
        var tokenData = _dbContext.Tokens.FirstOrDefault(x =>
            x.UserId == user.Id);

        if (tokenData == null)
        {
            var newToken = new TokenStorage(new Guid(), user.Id, token, DateTime.UtcNow.AddMinutes(30));
            await _dbContext.Tokens.AddAsync(newToken);
        }
        else
        {
            tokenData.ExpirationDate = DateTime.UtcNow.AddMinutes(30);
            tokenData.Token = token;
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User> GetUser(Guid userId)
    {
        if (userId == default)
        {
            throw new UnauthorizedAccessException("Token is expired or user unauthorized");
        }
        
        var user = _dbContext.Users
            .Include(p => p.Posts)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new ResourceNotFoundException("User is not found");
        }

        return user;
    }
    
    public async Task<User?> GetUserOrNull(Guid? userId)
    {
        if (userId == default(Guid) || userId == null)
        {
            return null;
        }
        
        var user = _dbContext.Users
            .Include(u => u.Posts)
            .FirstOrDefault(u => u.Id == userId);

        return user;
    }
}