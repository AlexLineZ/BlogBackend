using System.Runtime.InteropServices.JavaScript;
using BlogBackend.Data;
using BlogBackend.Models;
using BlogBackend.Models.DTO;
using BlogBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class CommunityService : ICommunityService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    
    public CommunityService(AppDbContext dbContext, IConfiguration configuration, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public async Task<List<CommunityDTO>> GetCommunity()
    {
        var communities = await _dbContext.Communities
            .ToListAsync();
        
        var communityDTOs = communities.Select(c => new CommunityDTO
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

    public async Task<List<CommunityUserDTO>> GetUserCommunity(String token)
    {
        var user = await GetUser(token);
        var userId = user.Id;
        
        var communityUserDTOs = _dbContext.Communities
            .Where(c => c.CommunityUsers.Any(cu => cu.UserId == user.Id))
            .Select(c => new CommunityUserDTO
            {
                UserId = user.Id,
                CommunityId = c.Id,
                Role = c.CommunityUsers.First(cu => cu.UserId == user.Id).Role
            })
            .ToList();
        
        return communityUserDTOs;
    }
    
    public async Task<CommunityFullDTO> GetCommunityById(Guid communityId)
    {
        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync();

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
                (cu, user) => new UserDTO
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
        
        var communityDTO = new CommunityFullDTO
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

    public async Task<CommunityRole> GetUserRole(Guid communityId, String token)
    {
        var user = await GetUser(token);
        var userId = user.Id;

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync();

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
            .FirstOrDefaultAsync();

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
        await _dbContext.SaveChangesAsync();
        return new OkResult();
    }

    public async Task<IActionResult> Unsubscribe(Guid communityId, String token)
    {
        var user = await GetUser(token);

        var community = await _dbContext.Communities
            .Include(c => c.CommunityUsers)
            .FirstOrDefaultAsync();

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
}