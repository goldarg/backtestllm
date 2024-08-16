using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Tiquetera
{
    public class TicketDto
    {
        public int id { get; set; }
        public string? nombreCompleto { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }
        public int empresaId { get; set; }
        public string? dominio { get; set; }
        public string? departamento { get; set; }
        public string? tipoOperacion { get; set; }
        public string? asunto { get; set; }
        public string? zona { get; set; }
        public string? descripcion { get; set; }
        public int odometro { get; set; }
        public DateTime turnoOpcion1 { get; set; }
        public DateTime turnoOpcion2 { get; set; }
        public string? idCRM { get; set; }
    }
}