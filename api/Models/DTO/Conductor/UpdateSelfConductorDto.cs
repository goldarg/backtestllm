using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO.Conductor;

public class UpdateSelfConductorDto
{
    [Phone(ErrorMessage = "El número de teléfono no es válido")]
    public required string Telefono { get; set; }
}
