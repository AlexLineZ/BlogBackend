﻿using System.ComponentModel.DataAnnotations;
using BlogBackend.Data.Models.User;
using BlogBackend.Validation;

namespace BlogBackend.Models;

public class UserRegisterModel
{
    [Required]
    [MinLength(1)]
    public String FullName { get; set; }
    
    [Required]
    [RegularExpression("^(?=.*\\d).{6,}$",
        ErrorMessage = "Password must be at least 6 letters and have at least 1 digit")]
    public String Password { get; set; }
    
    [Required]
    [RegularExpression("[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\\.[a-zA-Z0-9_-]+",
        ErrorMessage = "Invalid email address")]
    public String Email { get; set; }
    
    [BirthDateValidation]
    public DateTime? BirthDate { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [RegularExpression("^\\+7 \\(\\d{3}\\) \\d{3}-\\d{2}-\\d{2}$",
        ErrorMessage = "PhoneNumber is not valid")]
    public String PhoneNumber { get; set; }
}