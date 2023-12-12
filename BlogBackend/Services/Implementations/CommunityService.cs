using BlogBackend.Data;
using BlogBackend.Exceptions;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Models.Posts;
using BlogBackend.Services.Interfaces;
using BlogBackend.Validation;
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
        
        var communityDtOs = communities.Select(c => new CommunityDto
        {
            Id = c.Id,
            CreateTime = c.CreateTime,
            Name = c.Name,
            Description = c.Description,
            IsClosed = c.IsClosed,
            SubscribersCount = c.SubscribersCount
        }).ToList();

        return communityDtOs;
    }

    public async Task<List<CommunityUserDto>> GetUserCommunity(Guid userId)
    {
        var user = await _tokenService.GetUser(userId);

        var communityUserDtOs = _dbContext.Communities
            .Where(c => c.CommunityUsers.Any(cu => cu.UserId == user.Id))
            .Select(c => new CommunityUserDto
            {
                UserId = user.Id,
                CommunityId = c.Id,
                Role = c.CommunityUsers.First(cu => cu.UserId == user.Id).Role
            })
            .ToList();
        
        return communityUserDtOs;
    }
    
    public async Task<CommunityFullDto> GetCommunityById(Guid communityId)
    {
        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);

        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }

        var administrators =  community.CommunityUsers
            .Where(cu => cu.CommunityId == communityId && cu.Role == CommunityRole.Administrator)
            .Join(
                _dbContext.Users,
                cu => cu.UserId,
                user => user.Id,
                (_, user) => new UserDto
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
        
        var communityDto = new CommunityFullDto
        {
            Id = community.Id,
            CreateTime = community.CreateTime,
            Name = community.Name,
            Description = community.Description,
            IsClosed = community.IsClosed,
            SubscribersCount = community.SubscribersCount,
            Administrators = administrators
        };

        return communityDto;
    }

    public async Task<Guid> CreatePost(Guid communityId, CreatePostDto post, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);
        
        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => communityId == c.Id);
        
        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }
        
        var userRole = community.CommunityUsers
            .Where(cu => cu.CommunityId == communityId && cu.UserId == user.Id)
            .Select(cu => cu.Role)
            .FirstOrDefault();
        
        if (userRole != CommunityRole.Administrator || userRole == default)
        {
            throw new ResourceNotAccessException($"You are not administrator " +
                                                 $"of community with id: {communityId}");
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
            CommentsCount = 0,
            Tags = post.Tags
        };
        
        _dbContext.Posts.Add(newPost);
        community.Posts.Add(newPost);
        user.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();
        return newPost.Id;
    }

    public async Task<PostGroup> GetCommunityPost(Guid communityId, List<Guid>? tags,
        PostSorting? sorting, Int32 page, Int32 size, Guid userId)
    {
        QueryValidator.CheckValidDataCommunity(page, size);
        
        var community = await _dbContext.Communities
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == communityId);
        
        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }

        User? user = await _tokenService.GetUserOrNull(userId);

        var posts = community.Posts.AsQueryable();

        var filteredPosts = ApplyFilters(posts, tags, user, community);
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
                HasLike = user != null && user.Likes.Any(like => like == post.Id),
                CommentsCount = post.CommentsCount,
                Tags = _dbContext.Tags
                    .Where(tag => post.Tags.Contains(tag.Id))
                    .Select(tag => new TagDto
                    {
                        Id = tag.Id,
                        CreateTime = tag.CreateTime,
                        Name = tag.Name
                    })
                    .ToList(),
            }).ToList(),
                
            Pagination = new PageInfoModel 
                { Count = (int)Math.Ceiling((double)filteredPosts.Count() / size), Size = size, Current = page},
        };
        
        return postGroup;
    }

    public async Task<CommunityRole?> GetUserRole(Guid communityId, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);
        
        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }

        var userRole = community.CommunityUsers
            .Where(cu => cu.CommunityId == communityId && cu.UserId == user.Id)
            .Select(cu => cu.Role)
            .FirstOrDefault();

        if (userRole == default)
        {
            return null;
        }

        return userRole;
    }

    public async Task Subscribe(Guid communityId, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);

        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }
        
        var existingSubscription = community.CommunityUsers
            .FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == communityId);

        if (existingSubscription != null)
        {
            throw new InvalidOperationException("User is already subscribed" +
                                                $" to community with id: {communityId}");
        }
        
        var newSubscription = new CommunityUser
        {
            UserId = user.Id,
            CommunityId = communityId,
            Role = CommunityRole.Subscriber
        };

        community.CommunityUsers.Add(newSubscription);
        user.Communities.Add(communityId);
        community.SubscribersCount++;
        await _dbContext.SaveChangesAsync();
    }

    public async Task Unsubscribe(Guid communityId, Guid userId)
    {
        var user = await _tokenService.GetUser(userId);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync(c => c.Id == communityId);
        
        if (community == null)
        {
            throw new ResourceNotFoundException($"Community with id: {communityId} is not found");
        }
        
        var existingSubscription = community.CommunityUsers
            .FirstOrDefault(cu => cu.UserId == user.Id && cu.CommunityId == communityId);

        if (existingSubscription == null)
        {
            throw new InvalidOperationException($"User is not subscribed to community with id: {communityId}");
        }
        
        community.CommunityUsers.Remove(existingSubscription);
        community.SubscribersCount--;
        user.Communities.Remove(communityId);
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<Post> ApplyFilters(IQueryable<Post> posts, List<Guid>? tags, User? user, Community community)
    {
        if (user == null && community.IsClosed)
        {
            throw new ResourceNotAccessException($"User is not authenticated and " +
                                                 $"community with id: {community.Id} is closed");
        }

        if (user != null && community.IsClosed)
        {
            var isUserSubscribed = user.Communities.Any(c => c == community.Id);

            if (!isUserSubscribed)
            {
                throw new ResourceNotAccessException("User is not subscribed and " +
                    $"community with id: {community.Id} is closed");
            }
        }
        
        if (tags != null && tags.Any())
        {
            posts = posts.Where(p => p.Tags.Any(tags.Contains));
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
}

