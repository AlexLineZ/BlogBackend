using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
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
        PostSorting? sorting, bool onlyMyCommunities, int page, int size)
    {
        try
        {
            var allPosts = await _dbContext.Posts.ToListAsync();
            
            var filteredPosts = ApplyFilters(allPosts, tags, author, min, max, onlyMyCommunities);
            
            filteredPosts = ApplySorting(filteredPosts, sorting);
            
            var paginatedPosts = Paginate(filteredPosts, page, size);
            
            var postGroup = new PostGroup
            {
                Posts = paginatedPosts.Select(post => new PostDto
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
                    HasLike = post.HasLike,
                    CommentsCount = post.CommentsCount,
                    Tags = post.Tags
                }).ToList(),
                
                Pagination = new PageInfoModel { Count = size, Size = filteredPosts.Count(), Current = page},
            };
            
            return postGroup;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error retrieving posts: {ex.Message}");
        }
    }

    
    public async Task CreatePost(CreatePostDto post, String token)
    {
        var user = await GetUser(token);
        
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
            HasLike = false,
            CommentsCount = 0,
            Tags = post.Tags
        };
        
        _dbContext.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();
    }


    public async Task LikePost(Guid postId, String token)
    {
        var user = await GetUser(token);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new InvalidOperationException("Post not found");
        }
        
        var existingLike = user.Likes.Any(l => l == postId);
        
        if (existingLike)
        {
            throw new InvalidOperationException("You already like this post");
        }

        post.Likes++;
        user.Likes.Add(postId);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DislikePost(Guid postId, String token)
    {
        var user = await GetUser(token);
        
        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
        
        if (post == null)
        {
            throw new InvalidOperationException("Post not found");
        }
        
        var existingLike = user.Likes.FirstOrDefault(l => l == postId);
        
        if (existingLike == null)
        {
            throw new InvalidOperationException("You not like this post");
        }

        post.Likes--;
        user.Likes.Remove(postId);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<User> GetUser(String token)
    {
        var findToken = _dbContext.Tokens.FirstOrDefault(x =>
            token == x.Token);
        
        if (findToken != null)
        {
            if (_tokenService.IsTokenFresh(findToken) == false)
            {
                throw new InvalidOperationException("Token expired");
            }
        }
        
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == findToken.UserId);

        if (user == null)
        {
            throw new InvalidOperationException("User is not found");
        }

        return user;
    }
    
    private IQueryable<Post> ApplyFilters(List<Post> posts, List<Guid>? tags, string? author,
        int? minReadingTime, int? maxReadingTime, bool onlyMyCommunities)
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
        
        if (onlyMyCommunities)
        {
            //допилить
        }

        return filteredPosts;
    }

    private IQueryable<Post> ApplySorting(IQueryable<Post> posts, PostSorting? sorting)
    {
        return sorting switch
        {
            PostSorting.CreateDesk => posts.OrderByDescending(p => p.CreateTime),
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
}