using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
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

    public async Task CreatePost(CreatePostDto post, String token)
    {
        var user = await GetUser(token);
        
        var newPost = new Post {
            Id = new Guid(),
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
            Tags = await GetTagList(post.Tags)
        };
        
        _dbContext.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<List<TagDto>> GetTagList(List<Guid> list)
    {
        var tagDtos = new List<TagDto>();
        
        foreach (var item in list)
        {
            var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == item);
            tagDtos.Add(tag);
        }

        return tagDtos;
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
}