using BlogBackend.Data;
using BlogBackend.Models.DTO;
using BlogBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogBackend.Services.Implementations;

public class TagService: ITagService
{
    private readonly AppDbContext _dbContext;

    public TagService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<TagDto>> GetTagList()
    {
        var tags = await _dbContext.Tags.Select(tag => new TagDto
        {
            Id = tag.Id,
            CreateTime = tag.CreateTime,
            Name = tag.Name
        }).ToListAsync();
        
        return tags;
    }
}