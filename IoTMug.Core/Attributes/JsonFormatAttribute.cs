using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace IoTMug.Core.Attributes
{
    public class JsonFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try 
            { 
                JObject.Parse(value.ToString());
                return ValidationResult.Success;
            }
            catch { return new ValidationResult("Value cannot be parse as json."); }
        }
    }
}
