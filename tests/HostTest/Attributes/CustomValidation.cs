using System.ComponentModel.DataAnnotations;

namespace Nwpie.HostTest.Attributes
{
    public class CustomValidation : ValidationAttribute
    {
        public CustomValidation()
        {

        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }
}
