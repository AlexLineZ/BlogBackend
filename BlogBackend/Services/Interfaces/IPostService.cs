using BlogBackend.Data.Models.Posts;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;

namespace BlogBackend.Services.Interfaces;

public interface IPostService
{
    Task<PostGroup> GetPostList(List<Guid>? tags, String? author, Int32? min, Int32? max, PostSorting? sorting,
        Boolean onlyMyCommunities, Int32 page, Int32 size, String? token);
    Task<Guid> CreatePost(CreatePostDto post, String token);
    Task<PostFullDto> GetPost(Guid postId, String? token);
    Task LikePost(Guid postId, String token);
    Task DislikePost(Guid postId, String token);
}