using BlogBackend.Data;
using BlogBackend.Data.Models.Posts;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Models.Comments;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        PostSorting? sorting, bool onlyMyCommunities, int page, int size, string? token)
    {
        User? user = null;
        if (!token.IsNullOrEmpty())
        {
            user = await GetUserOrNull(token);
        }
            
        var posts = _dbContext.Posts
            .Include(p => p.Comments)
            .AsQueryable();
            
        posts = ApplyFilters(posts, tags, author, min, max, onlyMyCommunities, user);
            
        posts = ApplySorting(posts, sorting);

        var paginatedPosts = Paginate(posts, page, size);

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
                    HasLike = user != null && user.Posts.Contains(post.Id),
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
                
            Pagination = new PageInfoModel { Count = (int)Math.Ceiling((double)await posts.CountAsync() / size), Size = size, Current = page },
        };

        return postGroup;

    }

    public async Task<Guid> CreatePost(CreatePostDto post, String token)
    {
        var user = await _tokenService.GetUser(token);
        
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
        user.Posts.Add(newPost.Id);
        await _dbContext.SaveChangesAsync();
        return newPost.Id;
    }

    public async Task<PostFullDto> GetPost(Guid postId, string? token)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
        }

        User? user = null;
            
        if (!token.IsNullOrEmpty())
        {
            user = await GetUserOrNull(token);
        }

        var hasLike = user != null && user.Likes.Contains(postId);

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

    public async Task LikePost(Guid postId, String token)
    {
        var user = await _tokenService.GetUser(token);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
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
    
    public async Task DislikePost(Guid postId, String token)
    {
        var user = await _tokenService.GetUser(token);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new ResourceNotFoundException($"Post with id: {postId} not found");
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
        var filteredPosts = posts.AsQueryable();
        
        if (tags != null && tags.Any())
        {
            filteredPosts = filteredPosts.Where(p => p.Tags.Any(t => tags.Contains(t)));
        }
        
        if (!string.IsNullOrEmpty(author))
        {
            filteredPosts = filteredPosts.Where(p => p.Author.Contains(author));
        }
        
        if (minReadingTime.HasValue)
        {
            filteredPosts = filteredPosts.Where(p => p.ReadingTime >= minReadingTime.Value);
        }

        if (maxReadingTime.HasValue)
        {
            filteredPosts = filteredPosts.Where(p => p.ReadingTime <= maxReadingTime.Value);
        }
        
        if (onlyMyCommunities && user != null)
        {
            filteredPosts = filteredPosts.Where(p =>p.CommunityId != null
                                                    && user.Communities.Contains(p.CommunityId.Value));
        }

        if (!onlyMyCommunities || (onlyMyCommunities && user == null))
        {
            filteredPosts = filteredPosts
                .Where(post => !_dbContext.Communities
                    .Where(community => community.Id == post.CommunityId)
                    .Any(community => community.IsClosed && user != null && !user.Communities.Contains(community.Id))
                );
        }


        return filteredPosts;
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

        var commentTree = new List<Comment> { comment };

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
    
    private async Task<User?> GetUserOrNull(string? token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);

        if (findToken == null)
        {
            return null;
        }
        
        if (_tokenService.IsTokenFresh(findToken) == false)
        {
            return null;
        }
        
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == findToken.UserId);

        return user;
    }
}