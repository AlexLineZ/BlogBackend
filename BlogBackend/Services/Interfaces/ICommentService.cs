using BlogBackend.Models.Comments;

namespace BlogBackend.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsTree(Guid commentId);
    Task CreateComment(CreateCommentDto comment, Guid postId, string token);
    Task UpdateComment(Guid commentId, UpdateCommentDto comment, string token);
    Task DeleteComment(Guid commentId, string token);
}