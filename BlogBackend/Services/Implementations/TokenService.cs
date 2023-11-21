using BlogBackend.Data;
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
}