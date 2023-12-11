using BlogBackend.Models.DTO;

namespace BlogBackend.Services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAuthorList();
}