using BlogBackend.Data.Models.Posts;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;

namespace BlogBackend.Services.Interfaces;

public interface IPostService
{
    Task<PostGroup> GetPostList(List<Guid>? tags, String? author, Int32? min, Int32? max, PostSorting? sorting,
        Boolean onlyMyCommunities, Int32 page, Int32 size, Guid? userId);
    Task<Guid> CreatePost(CreatePostDto post, Guid userId);
    Task<PostFullDto> GetPost(Guid postId, Guid? userId);
    Task LikePost(Guid postId, Guid userId);
    Task DislikePost(Guid postId, Guid userId);
}