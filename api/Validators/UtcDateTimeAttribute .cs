using System.ComponentModel.DataAnnotations;

namespace api.Validators;

public class UtcDateTimeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime && dateTime.Kind != DateTimeKind.Utc)
        {
            return new ValidationResult("El valor debe estar en formato UTC.");
        }
        return ValidationResult.Success;
    }
}
