using BlogBackend.Models;

namespace BlogBackend.Services.Interfaces;

public interface ITokenService
{
    Task AddOrEditToken(String token, User user);
    Boolean IsTokenFresh(TokenStorage tokenData);
    Task<User> GetUser(Guid userId);
}