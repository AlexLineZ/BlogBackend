using BlogBackend.Data;
using BlogBackend.Data.Models.Posts;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Models.Comments;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using BlogBackend.Validation;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class PostService: IPostService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public PostService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<PostGroup> GetPostList(List<Guid>? tags, string? author, int? min, int? max,
        PostSorting? sorting, bool onlyMyCommunities, int page, int size, Guid? userId)
    {
        QueryValidator.CheckValidDataPost(page, size, min, max);
        User? user = await _tokenService.GetUserOrNull(userId);

        var posts = _dbContext.Posts
            .Include(p => p.Comments)
            .AsQueryable();
            
        posts = ApplyFilters(posts, tags, author, min, max, onlyMyCommunities, user);
            
        posts = ApplySorting(posts, sorting);

        var paginatedPosts = Paginate(posts, page, size);

        var pagesCount = (int)Math.Ceiling((double)posts.Count() / size);

        if (pagesCount < page)
        {
            throw new InvalidOperationException("Invalid number of page");
        }
        
        var postGroup = new PostGroup
        {
            Posts = await paginatedPosts
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    CreateTime = post.CreateTime,
                    Title = post.Title,
                    Description = post.Description,
                    ReadingTime = post.ReadingTime,
                    Image = post.Image,
                    AuthorId = post.AuthorId,
                    Author = post.Author,
                    CommunityId = post.CommunityId,
                    CommunityName = post.CommunityName,
                    AddressId = post.AddressId,
                    Likes = post.Likes,
                    HasLike = user != null && user.Posts.Contains(post),
                    CommentsCount = post.Comments.Count,
                    Tags = _dbContext.Tags
                        .Where(tag => post.Tags.Contains(tag.Id))
                        .Select(tag => new TagDto
                        {
                            Id = tag.Id,
                            CreateTime = tag.CreateTime,
                            Name = tag.Name
                        })
                        .ToList()
                })
                .ToListAsync(),
                
            Pagination = new PageInfoModel
                { Count = pagesCount, Size = size, Current = page },
        };

        return postGroup;

    }

    public async Task<Guid> CreatePost(CreatePostDto post, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);

        if (!await TagsExist(post.Tags))
        {
            throw new ResourceNotFoundException("One or more tags do not exist");
        }
        
        var newPost = new Post {
            Id = Guid.NewGuid(),
            CreateTime = DateTime.UtcNow, 
            Title = post.Title,
            Description = post.Description,
            ReadingTime = post.ReadingTime,
            Image = post.Image,
            AuthorId = user.Id,
            Author = user.FullName,
            CommunityId = null,
            CommunityName = null,
            AddressId = post.AddressId,
            Likes = 0,
            CommentsCount = 0,
            Tags = post.Tags,
            Comments = new List<Comment>()
        };
        
        _dbContext.Posts.Add(newPost);
        user.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();
        return newPost.Id;
    }
    
    public async Task<PostFullDto> GetPost(Guid postId, Guid? userId)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
        }

        User? user = await _tokenService.GetUserOrNull(userId);
        
        var hasLike = user != null && user.Likes.Contains(postId);

        if (post.CommunityId != null)
        {
            IsPostAvailable(post.CommunityId.Value, user);
        }
        
        var comments = post.Comments
            .Where(c => c.ParentId == null)
            .Select(comment => new CommentDto
            {
                Id = comment.Id,
                CreateTime = comment.CreateTime,
                Content = comment.Content,
                ModifiedDate = comment.ModifiedDate,
                DeleteDate = comment.DeleteDate,
                AuthorId = comment.AuthorId,
                Author = comment.Author,
                SubComments = BuildCommentTree(comment.Id).Count - 1
            })
            .ToList();
        
        var postDto = new PostFullDto
        {
            Id = post.Id,
            CreateTime = post.CreateTime,
            Title = post.Title,
            Description = post.Description,
            ReadingTime = post.ReadingTime, 
            Image = post.Image,
            AuthorId = post.AuthorId,
            Author = post.Author,
            CommunityId = post.CommunityId,
            CommunityName = post.CommunityName,
            AddressId = post.AddressId,
            Likes = post.Likes,
            HasLike = hasLike,
            CommentsCount = post.Comments.Count,
            Tags = _dbContext.Tags
                .Where(tag => post.Tags.Contains(tag.Id))
                .Select(tag => new TagDto
                {
                    Id = tag.Id,
                    CreateTime = tag.CreateTime,
                    Name = tag.Name
                })
                .ToList(),
            Comments = comments
        };

        return postDto;
    }

    public async Task LikePost(Guid postId, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
        }
        
        if (post.CommunityId != null)
        {
            IsPostAvailable(post.CommunityId.Value, user);
        }
        
        var existingLike = user.Likes.Any(l => l == postId);
        
        if (existingLike)
        {
            throw new InvalidOperationException($"You already like post with id: {postId}");
        }

        post.Likes++;
        user.Likes.Add(postId);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DislikePost(Guid postId, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
        }
        
        if (post.CommunityId != null)
        {
            IsPostAvailable(post.CommunityId.Value, user);
        }
        
        var existingLike = user.Likes.Any(l => l == postId);
        
        if (!existingLike)
        {
            throw new InvalidOperationException($"You not like post with id: {postId}");
        }

        post.Likes--;
        user.Likes.Remove(postId);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<Post> ApplyFilters(IQueryable<Post> posts, List<Guid>? tags, string? author,
        int? minReadingTime, int? maxReadingTime, bool onlyMyCommunities, User? user)
    {
        if (tags != null && tags.Any())
        {
            var tagList = tags.ToList();
            posts = posts.Where(p => p.Tags.Any(t => tagList.Contains(t)));
        }
        
        if (!string.IsNullOrEmpty(author))
        {
            posts = posts.Where(p => p.Author.ToUpper().Contains(author.ToUpper()));
        }

        if (minReadingTime.HasValue)
        {
            posts = posts.Where(p => p.ReadingTime >= minReadingTime.Value);
        }

        if (maxReadingTime.HasValue)
        {
            posts = posts.Where(p => p.ReadingTime <= maxReadingTime.Value);
        }
        
        if (onlyMyCommunities && user != null)
        {
            posts = posts
                .Where(p => p.CommunityId != null
                                  && user.Communities.Any(c => c == p.CommunityId.Value));
        }
        
        if (onlyMyCommunities && user == null || user == null)
        {
            posts = posts
                .Where(post => !_dbContext.Communities
                    .Where(community => community.Id == post.CommunityId)
                    .Any(community => community.IsClosed)
                );
        }

        if (!onlyMyCommunities && user != null)
        {
            posts = posts
                .Where(post =>
                    !_dbContext.Communities
                        .Where(community => community.Id == post.CommunityId)
                        .Any(community => community.IsClosed && !user.Communities.Contains(community.Id))
                );
        }

        return posts;
    }

    private IQueryable<Post> ApplySorting(IQueryable<Post> posts, PostSorting? sorting)
    {
        return sorting switch
        {
            PostSorting.CreateDesс => posts.OrderByDescending(p => p.CreateTime),
            PostSorting.CreateAsc => posts.OrderBy(p => p.CreateTime),
            PostSorting.LikeAsc => posts.OrderBy(p => p.Likes),
            PostSorting.LikeDesc => posts.OrderByDescending(p => p.Likes),
            _ => posts,
        };
    }

    private IQueryable<Post> Paginate(IQueryable<Post> posts, int page, int size)
    {
        return posts.Skip((page - 1) * size).Take(size);
    }

    private List<Comment> BuildCommentTree(Guid commentId)
    {
        var comment = _dbContext.Comments
            .Include(c => c.SubCommentsList)
            .FirstOrDefault(c => c.Id == commentId);

        if (comment == null)
        {
            throw new ResourceNotFoundException($"Comment with id: {commentId} not found");
        }
        
        var commentTree = new List<Comment> { comment };

        foreach (var subComment in comment.SubCommentsList)
        {
            var subCommentTree = BuildCommentTree(subComment.Id);
            commentTree.AddRange(subCommentTree);
        }
        
        return commentTree;
    }

    private Boolean IsPostAvailable(Guid communityId, User? user)
    {
        if (user == null)
        {
            throw new ResourceNotAccessException($"Community with id: {communityId} is closed");
        }
        
        var community = _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefault(p => p.Id == communityId);

        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} not found");
        }
        
        if (community.IsClosed)
        {
            var isUserSubscribed =  community.CommunityUsers.Any(c => c.UserId == user.Id);
            if (!isUserSubscribed)
            {
                throw new ResourceNotAccessException($"User is not subscribed " +
                                                     $"to community with id: {communityId}");
            }
        }

        return true;
    }
    
    private async Task<bool> TagsExist(List<Guid> tagIds)
    {
        var existingTags = await _dbContext.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync();

        return existingTags.Count == tagIds.Count;
    }
}