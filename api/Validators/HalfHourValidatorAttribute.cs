using System.ComponentModel.DataAnnotations;

namespace api.Validators;

public class HalfHourValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Minute != 0 && dateTime.Minute != 30)
            {
                return new ValidationResult("Los minutos deben ser 00 o 30.");
            }

            if (dateTime.Second != 0 || dateTime.Millisecond != 0)
            {
                return new ValidationResult("Los segundos y milisegundos deben ser 00.");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("El valor no es un DateTime válido.");
    }
}
