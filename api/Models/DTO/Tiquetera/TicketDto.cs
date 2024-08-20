using System.ComponentModel.DataAnnotations;
using api.Validators;

namespace api.Models.DTO.Tiquetera
{
    public class TicketDto
    {
        [EmailAddress]
        public required string email { get; set; }

        [Phone]
        public required string telefono { get; set; }
        public required int empresaId { get; set; }
        public required string empresaNombre { get; set; }

        // este es el dominio, no su id, ej: "AG055YT"
        public required string dominio { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "El campo solo números.")]
        public required string departamentoId { get; set; }
        public required string tipoOperacion { get; set; }
        public required string zona { get; set; }
        public string? descripcion { get; set; }
        public required int odometro { get; set; }

        [HalfHourValidator]
        public required DateTime turnoOpcion1 { get; set; }

        [HalfHourValidator]
        public required DateTime turnoOpcion2 { get; set; }
    }
}
