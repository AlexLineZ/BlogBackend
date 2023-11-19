using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Data.Models.User;

public class TokenResponse
{
    [Required]
    [MinLength(1)]
    public String Token;
}