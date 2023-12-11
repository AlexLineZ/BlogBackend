using System.ComponentModel.DataAnnotations;
using BlogBackend.Data;

namespace BlogBackend.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidAddressIdAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        
        var serviceProvider = validationContext.GetRequiredService<IServiceProvider>();
        var dbContext = serviceProvider.GetRequiredService<Gar70Context>();
        
        var addressId = (Guid)value;

        var isAddressExists = dbContext.AsAddrObjs.Any(a => a.Objectguid == addressId) 
                              || dbContext.AsHouses.Any(a => a.Objectguid == addressId);

        if (!isAddressExists)
        {
            return new ValidationResult(ErrorMessage ?? "Invalid addressId");
        }

        return ValidationResult.Success;
    }
}