using System.ComponentModel.DataAnnotations;
using api.Attributes;

namespace api.Models.DTO.User
{
    public class UserDto
    {
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres")]
        public required string Nombre { get; set; }

        [MinLength(2, ErrorMessage = "El apellido debe tener al menos 2 caracteres")]
        public required string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido")]
        public required string Telefono { get; set; }

        [ValidCargo]
        public required string Puesto { get; set; }

        [MinLength(1, ErrorMessage = "Debe haber al menos una empresa en la lista")]
        public required List<string> EmpresasIdsCrm { get; set; }
        public required int RolId { get; set; }

        // TODO poder asignar al mismo tiempo un auto
    }
}
