﻿using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Models.Comments;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class CommentService: ICommentService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public CommentService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<List<CommentDto>> GetCommentsTree(Guid commentId)
    {
        var rootComment = await _dbContext.Comments
            .Include(c => c.SubCommentsList)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (rootComment == null)
        {
            throw new ResourceNotFoundException("Comment not found");
        }

        if (rootComment.ParentId != null)
        {
            throw new InvalidOperationException("Comment is not a root comment");
        }

        var commentTree = BuildCommentTree(rootComment.Id);

        commentTree.RemoveAt(0);
        return commentTree;
    }

    public async Task CreateComment(CreateCommentDto commentDto, Guid postId, string token)
    {
        var user = await _tokenService.GetUser(token);

        var post = await _dbContext.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new ResourceNotFoundException("Post not found");
        }

        var newComment = new Comment
        {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow,
            Content = commentDto.Content,
            AuthorId = user.Id,
            Author = user.FullName,
            SubComments = 0,
            SubCommentsList = new List<Comment>(),
            ParentId = commentDto.ParentId,
            PostId = postId
        };

        _dbContext.Comments.Add(newComment);

        if (commentDto.ParentId.HasValue)
        {
            var parentComment = await _dbContext.Comments
                    .Include(c => c.SubCommentsList)
                    .FirstOrDefaultAsync(c => c.Id == commentDto.ParentId.Value);
            if (parentComment == null || parentComment.PostId != postId)
            {
                throw new ResourceNotFoundException("Parent comment not found");
            }

            parentComment.SubCommentsList.Add(newComment);
            parentComment.SubComments++;
        }
        else
        {
            post.Comments.Add(newComment);
        }

        await _dbContext.SaveChangesAsync();
    }


    public async Task UpdateComment(Guid commentId, UpdateCommentDto comment, string token)
    {
        var existingComment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (existingComment == null)
        {
            throw new ResourceNotFoundException("Comment not found");
        }

        var user = await _tokenService.GetUser(token);

        if (existingComment.AuthorId != user.Id)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this comment");
        }

        existingComment.Content = comment.Content;
        existingComment.ModifiedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteComment(Guid commentId, string token)
    {
        var user = await _tokenService.GetUser(token);
        
        var commentToDelete = await _dbContext.Comments
            .Include(c => c.SubCommentsList)
            .FirstOrDefaultAsync(c => c.Id == commentId);
        
        if (commentToDelete == null)
        {
            throw new ResourceNotFoundException("Comment not found");
        }
        
        if (commentToDelete.AuthorId != user.Id && !await IsUserCommunityAdmin(user, commentToDelete.PostId))
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this comment");
        }
        
        if (commentToDelete.ParentId == null && commentToDelete.SubCommentsList.Count == 0)
        {
            _dbContext.Comments.Remove(commentToDelete);
        }
        else
        {
            commentToDelete.DeleteDate = DateTime.UtcNow;
        }
        
        await _dbContext.SaveChangesAsync();
    }

    private async Task<bool> IsUserCommunityAdmin(User user, Guid? postId)
    {
        if (postId == null)
        {
            return false;
        }
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new ResourceNotFoundException("Post not found");
        }

        if (post.CommunityId == null)
        {
            return false;
        }
        
        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == post.CommunityId);
        
        if (community == null)
        {
            throw new ResourceNotFoundException("Community not found");
        }
        
        var userRole = community.CommunityUsers
            .Where(cu => cu.CommunityId == community.Id && cu.UserId == user.Id)
            .Select(cu => cu.Role)
            .FirstOrDefault();

        if (userRole == default)
        {
            return false;
        }

        return true;
    }

    
    private List<CommentDto> BuildCommentTree(Guid commentId)
    {
        var comment = _dbContext.Comments
            .Include(c => c.SubCommentsList)
            .FirstOrDefault(c => c.Id == commentId);

        if (comment == null)
        {
            throw new ResourceNotFoundException("Comment not found");
        }
        
        var commentTree = new List<CommentDto>
        {
            MapCommentToDto(comment)
        };

        if (comment.SubCommentsList != null)
        {
            foreach (var subComment in comment.SubCommentsList)
            {
                var subCommentTree = BuildCommentTree(subComment.Id);
                commentTree.AddRange(subCommentTree);
            }
        }

        return commentTree;
    }


    private CommentDto MapCommentToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            CreateTime = comment.CreateTime,
            Content = comment.Content,
            ModifiedDate = comment.ModifiedDate,
            DeleteDate = comment.DeleteDate,
            AuthorId = comment.AuthorId,
            Author = comment.Author,
            SubComments = comment.SubComments
        };
    }
}