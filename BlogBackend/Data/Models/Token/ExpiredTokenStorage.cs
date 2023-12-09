using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Models;

public class ExpiredTokenStorage
{
    [Key]
    public String Token { get; set; }
}