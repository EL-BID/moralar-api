
using System.ComponentModel.DataAnnotations;

namespace Moralar.UtilityFramework.Application.Core
{
    public class IsValidCpf : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString().ValidCpf())
            {
                return null;
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
