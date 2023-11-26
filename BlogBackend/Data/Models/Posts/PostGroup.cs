using BlogBackend.Models.DTO;

namespace BlogBackend.Models.Posts;

public class PostGroup
{
    public List<PostDto> Posts { get; set; }
    public PageInfoModel Pagination { get; set; }
}