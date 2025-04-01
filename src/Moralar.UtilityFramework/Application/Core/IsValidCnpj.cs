using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Application.Core
{
    public class IsValidCnpj : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString().ValidCnpj())
            {
                return null;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
