using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models.DTO;

public class TagDto
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; }

    public TagDto() { }
    
    public TagDto(Guid id, DateTime createTime, string name)
    {
        Id = id;
        CreateTime = createTime;
        Name = name;
    }
}