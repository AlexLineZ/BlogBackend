using BlogBackend.Models.DTO;

namespace BlogBackend.Services.Interfaces;

public interface IPostService
{
    Task CreatePost(CreatePostDto post, String token);
}