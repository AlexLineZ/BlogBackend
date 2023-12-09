using BlogBackend.Data;
using BlogBackend.Services.Interfaces;

namespace BlogBackend.Services.Implementations;

public class BannedTokenService : IBannedTokenService
{
    private readonly AppDbContext _dbContext;

    public BannedTokenService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<bool> IsTokenBannedAsync(string token)
    {
        throw new NotImplementedException();
    }
}