using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly AppDbContext _dbContext;
    
    public TokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public Boolean IsTokenFresh(TokenStorage tokenData)
    {
        return DateTime.UtcNow <= tokenData.ExpirationDate;
    }
    
    public async Task<User> GetUser(Guid userId)
    {
        if (userId == default)
        {
            throw new UnauthorizedAccessException("Token is expired or user unauthorized");
        }
        
        var user = _dbContext.Users
            .Include(p => p.Posts)
            .Include(c => c.Communities)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            throw new ResourceNotFoundException("User is not found");
        }

        return user;
    }
}