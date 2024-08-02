using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities
{
    public class ActividadUsuario
    {
        public int id { get; set; }
        public string? descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int usuarioEjecutorId { get; set; }
        public int usuarioAfectadoId { get; set; }
        public virtual User? usuarioEjecutor { get; set; }
        public virtual User? usuarioAfectado { get; set; }
    }
}
