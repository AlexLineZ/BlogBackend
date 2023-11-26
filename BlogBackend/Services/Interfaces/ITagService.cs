using BlogBackend.Models.DTO;

namespace BlogBackend.Services.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetTagList();
}