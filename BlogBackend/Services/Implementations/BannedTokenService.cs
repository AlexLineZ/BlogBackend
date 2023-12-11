using BlogBackend.Data;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class BannedTokenService : IBannedTokenService
{
    private readonly AppDbContext _dbContext;

    public BannedTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<bool> IsTokenBannedAsync(string token)
    {
        var checkToken = await _dbContext.ExpiredTokens
            .FirstOrDefaultAsync(x => x.Token == token);
        
        if (checkToken != null)
        {
            return true;
        }
        
        return false;
    }
}