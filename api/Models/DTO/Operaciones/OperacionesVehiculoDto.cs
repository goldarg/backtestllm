using System.Text.Json.Serialization;

namespace api.Models.DTO.Operaciones
{
    public class OperacionesVehiculoDto
    {
        public string? Id { get; set; }
        public string? TipoOperacion { get; set; }
        public List<string>? Detalle { get; set; }
        public string? Taller { get; set; }
        public DateTime? FechaTurno { get; set; }
        public string? Estado { get; set; }
        public string? OT { get; set; }
    }
}