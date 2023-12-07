using BlogBackend.Data;
using BlogBackend.Models.DTO;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class AuthorService: IAuthorService
{
    private readonly AppDbContext _dbContext;

    public AuthorService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AuthorDto>> GetAuthorList()
    {
        var authors = await _dbContext.Users
            .Select(author => new AuthorDto
            {
                FullName = author.FullName,
                BirthDate = author.BirthDate,
                Gender = author.Gender,
                Posts = author.Posts.Count,
                Likes = author.Posts
                        .Where(post => author.Id == post.AuthorId)
                        .Sum(post => post.Likes),
                Created = author.CreateTime
            })
            .ToListAsync();

        return authors;
    }

}