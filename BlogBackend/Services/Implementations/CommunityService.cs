using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class CommunityService : ICommunityService
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;
    
    public CommunityService(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<List<CommunityDto>> GetCommunity()
    {
        var communities = await _dbContext.Communities
            .ToListAsync();
        
        var communityDTOs = communities.Select(c => new CommunityDto
        {
            Id = c.Id,
            CreateTime = c.CreateTime,
            Name = c.Name,
            Description = c.Description,
            IsClosed = c.IsClosed,
            SubscribersCount = c.SubscribersCount
        }).ToList();

        return communityDTOs;
    }

    public async Task<List<CommunityUserDto>> GetUserCommunity(String token)
    {
        var user = await GetUser(token);

        var communityUserDTOs = _dbContext.Communities
            .Where(c => c.CommunityUsers.Any(cu => cu.UserId == user.Id))
            .Select(c => new CommunityUserDto
            {
                UserId = user.Id,
                CommunityId = c.Id,
                Role = c.CommunityUsers.First(cu => cu.UserId == user.Id).Role
            })
            .ToList();
        
        return communityUserDTOs;
    }
    
    public async Task<CommunityFullDto> GetCommunityById(Guid communityId)
    {
        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);

        if (community == null)
        {
            throw new FileNotFoundException("Community is not found");
        }

        var administrators =  community.CommunityUsers
            .Where(cu => cu.CommunityId == communityId && cu.Role == CommunityRole.Administrator)
            .Join(
                _dbContext.Users,
                cu => cu.UserId,
                user => user.Id,
                (cu, user) => new UserDto
                {
                    Id = user.Id,
                    CreateTime = user.CreateTime,
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            ).ToList();
        
        var communityDTO = new CommunityFullDto
        {
            Id = community.Id,
            CreateTime = community.CreateTime,
            Name = community.Name,
            Description = community.Description,
            IsClosed = community.IsClosed,
            SubscribersCount = community.SubscribersCount,
            Administrators = administrators
        };

        return communityDTO;
    }

    public async Task CreatePost(Guid communityId, CreatePostDto post, String token)
    {
        var user = await GetUser(token);
        
        var community = await _dbContext.Communities
            .FirstOrDefaultAsync(c => communityId == c.Id);
        
        if (community == null)
        {
            throw new FileNotFoundException("Community is not found");
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
            CommunityId = community.Id,
            CommunityName = community.Name,
            AddressId = post.AddressId,
            Likes = 0,
            HasLike = false,
            CommentsCount = 0,
            Tags = post.Tags
        };
        
        _dbContext.Posts.Add(newPost);
        community.Posts.Add(newPost.Id);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PostGroup> GetCommunityPost(Guid communityId, List<Guid>? tags,
        PostSorting? sorting, Int32 page, Int32 size)
    {
        var community = await _dbContext.Communities
            .FirstOrDefaultAsync(c => c.Id == communityId);
        
        if (community == null)
        {
            throw new FileNotFoundException("Community is not found");
        }
        
        var posts = new List<Post>();
        community.Posts.ForEach(postId =>
        {
            var post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                posts.Add(post);
            }
        });

        var filteredPosts = ApplyFilters(posts, tags);
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

    public async Task<CommunityRole> GetUserRole(Guid communityId, String token)
    {
        var user = await GetUser(token);
        var userId = user.Id;

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);

        var userRole = community.CommunityUsers
            .Where(cu => cu.CommunityId == communityId && cu.UserId == userId)
            .Select(cu => cu.Role)
            .FirstOrDefault();

        if (userRole == null)
        {
            throw new InvalidOperationException("User is not a member of the community");
        }

        return userRole;
    }

    public async Task<IActionResult> Subscribe(Guid communityId, String token)
    {
        var user = await GetUser(token);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);

        if (community == null)
        {
            throw new FileNotFoundException("Community is not found");
        }
        
        var existingSubscription = community.CommunityUsers
            .FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == communityId);

        if (existingSubscription != null)
        {
            throw new InvalidOperationException("User is already subscribed to this community");
        }
        
        var newSubscription = new CommunityUser
        {
            UserId = user.Id,
            CommunityId = communityId,
            Role = CommunityRole.Subscriber
        };

        community.CommunityUsers.Add(newSubscription);
        user.Communities.Add(newSubscription.CommunityId);
        community.SubscribersCount++;
        await _dbContext.SaveChangesAsync();
        return new OkResult();
    }

    public async Task<IActionResult> Unsubscribe(Guid communityId, String token)
    {
        var user = await GetUser(token);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);
        
        if (community == null)
        {
            throw new FileNotFoundException("Community is not found");
        }
        
        var existingSubscription = community.CommunityUsers
            .FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == communityId);

        if (existingSubscription == null)
        {
            throw new InvalidOperationException("User is not subscribed to this community");
        }
        
        community.CommunityUsers.Remove(existingSubscription);
        community.SubscribersCount--;
        user.Communities.Remove(existingSubscription.CommunityId);
        await _dbContext.SaveChangesAsync();
        return new OkResult();
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

    private IQueryable<Post> ApplyFilters(List<Post> posts, List<Guid>? tags)
    {
        var filteredPosts = posts.AsQueryable();
        
        if (tags != null && tags.Any())
        {
            filteredPosts = filteredPosts.Where(p => p.Tags.Any(t => tags.Contains(t)));
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

