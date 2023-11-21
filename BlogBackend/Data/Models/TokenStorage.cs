using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogBackend.Data.Models.User;

namespace BlogBackend.Models;

public class TokenStorage
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }           
    [Required]
    public String Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    
    public TokenStorage() {}
    public TokenStorage(Guid id, Guid userId, String token, DateTime expirationDate)
    {
        Id = id;
        UserId = userId;
        Token = token;
        ExpirationDate = expirationDate;
    }
}