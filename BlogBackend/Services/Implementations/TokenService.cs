using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Services.Interfaces;

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
    
    public async Task<User> GetUser(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);

        if (findToken == null)
        {
            throw new UnauthorizedAccessException("Token is not found");
        }
        
        if (IsTokenFresh(findToken) == false)
        {
            throw new UnauthorizedAccessException("Token expired");
        }
        
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == findToken.UserId);

        if (user == null)
        {
            throw new ResourceNotFoundException("User is not found");
        }

        return user;
    }
}