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
        public required string empresaCrmId { get; set; }
        public required string empresaNombre { get; set; }
        public required string dominioCrmId { get; set; }
        public required string dominio { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "El campo solo números.")]
        public required string departamentoCrmId { get; set; }
        public required string tipoOperacion { get; set; }
        public required string zona { get; set; }
        public required string descripcion { get; set; }
        public required int odometro { get; set; }

        [HalfHourValidator]
        public required DateTime turnoOpcion1 { get; set; }

        [HalfHourValidator]
        public required DateTime turnoOpcion2 { get; set; }
    }
}
