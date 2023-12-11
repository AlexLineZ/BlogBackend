using System.ComponentModel.DataAnnotations;

namespace BlogBackend.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class BirthDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        
        if (value is DateTime birthDate)
        {
            if (birthDate > DateTime.UtcNow.Date)
            {
                return new ValidationResult(ErrorMessage ?? "Birth date can't be later than today");
            }
        }
        
        return ValidationResult.Success;
    }
}