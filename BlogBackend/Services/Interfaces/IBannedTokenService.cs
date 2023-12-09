namespace BlogBackend.Services.Interfaces;

public interface IBannedTokenService
{
    Task<bool> IsTokenBannedAsync(string token);
}