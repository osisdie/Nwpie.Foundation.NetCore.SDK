using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Nwpie.xUnit.Foundation.Validation
{
    public class ClientAddressInfo
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Bookkeeping")]
        public bool Bookkeeping { get; set; }

        [Required]
        [DisplayName("Personal Income Taxation")]
        public bool Personal_Income_Taxation { get; set; }

        [Required]
        [DisplayName("Self-Employed Business Taxes")]
        public bool Self_Employed_Business_Taxes { get; set; }

        [Required]
        [DisplayName("GST/PST/WCB Returns")]
        public bool GST_PST_WCB_Returns { get; set; }

        [Required]
        [DisplayName("Tax Returns")]
        public bool Tax_Returns { get; set; }

        [Required]
        [DisplayName("Payroll Services")]
        public bool Payroll_Services { get; set; }

        [Required]
        [DisplayName("Previous Year Filings")]
        public bool Previous_Year_Filings { get; set; }

        [Required]
        [DisplayName("Govt. Requisite Form Applicaitons")]
        public bool Government_Requisite_Form_Applications { get; set; }

        public string Other { get; set; }

        [CheckboxAndOtherValidation(nameof(Bookkeeping),
            nameof(Personal_Income_Taxation),
            nameof(Self_Employed_Business_Taxes),
            nameof(GST_PST_WCB_Returns),
            nameof(Tax_Returns),
            nameof(Payroll_Services),
            nameof(Previous_Year_Filings),
            nameof(Government_Requisite_Form_Applications))]
        public bool AreCheckboxesAndOtherValid { get; set; }
    }

    public class CheckboxAndOtherValidation : ValidationAttribute
    {
        private readonly object TRUE = true;
        private readonly string[] m_AlltheOtherProperty;

        public CheckboxAndOtherValidation(params string[] alltheOthersProperty)
        {
            m_AlltheOtherProperty = alltheOthersProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (m_AlltheOtherProperty?.Count() > 0 != true)
            {
                return ValidationResult.Success;
            }

            var otherPropertyInfo = validationContext.ObjectType.GetProperty(nameof(ClientAddressInfo.Other));
            if (otherPropertyInfo != null)
            {
                var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
                if (otherPropertyValue != null && !string.IsNullOrEmpty(otherPropertyValue.ToString()))
                {
                    return ValidationResult.Success;
                }
            }

            for (var i = 0; i < m_AlltheOtherProperty.Length; ++i)
            {
                var prop = m_AlltheOtherProperty[i];
                var propertyInfo = validationContext.ObjectType.GetProperty(prop);
                if (propertyInfo == null)
                {
                    continue;
                }

                var propertyValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
                if (Equals(TRUE, propertyValue))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("Must exist at least one field is true", m_AlltheOtherProperty);
        }
    }
}
