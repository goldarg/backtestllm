using System.ComponentModel.DataAnnotations;
using api.Configuration;

namespace api.Attributes;

public class ValidCargoAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("El puesto es requerido.");
        }

        if (value is string puesto && CargoOptions.OpcionesValidas.Contains(puesto))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("El puesto no es válido.");
    }
}
