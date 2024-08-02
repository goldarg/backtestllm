using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.ActividadUsuarios
{
    public class ActividadUsuarioDto
    {
        public int id { get; set; }
        public string? descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int usuarioEjecutorId { get; set; }
        public int usuarioAfectadoId { get; set; }
        public string usuarioEjecutorNombre { get; set; }
        public string usuarioAfectadoNombre { get; set; }
    }
}
