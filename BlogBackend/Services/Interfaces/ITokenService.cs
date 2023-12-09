using BlogBackend.Models;

namespace BlogBackend.Services.Interfaces;

public interface ITokenService
{
    String GenerateJwtToken(IConfiguration configuration, User user);
    Task AddOrEditToken(String token, User user);
    Task<User> GetUser(Guid userId);
    Task<User?> GetUserOrNull(Guid? userId);
}