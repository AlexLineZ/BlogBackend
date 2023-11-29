using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;

namespace BlogBackend.Services.Interfaces;

public interface IPostService
{
    Task<PostGroup> GetPostList(List<Guid>? tags, String? author, Int32? min, Int32? max, PostSorting? sorting,
        Boolean onlyMyCommunities, Int32 page, Int32 size);
    Task CreatePost(CreatePostDto post, String token);

    Task LikePost(Guid postId, String token);
    
    Task DislikePost(Guid postId, String token);
}