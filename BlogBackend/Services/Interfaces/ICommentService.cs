using BlogBackend.Models.Comments;

namespace BlogBackend.Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentsTree(Guid commentId, Guid userId);
    Task<Guid> CreateComment(CreateCommentDto comment, Guid postId, Guid userId);
    Task UpdateComment(Guid commentId, UpdateCommentDto comment, Guid userId);
    Task DeleteComment(Guid commentId, Guid userId);
}