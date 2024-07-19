using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities
{
    public class Empresa
    {
        public int id { get; set; }
        public string? razonSocial { get; set; }
        public Guid guid { get; set;}
        public string? idCRM { get; set; }
        public virtual ICollection<UsuariosEmpresas>? Asignaciones { get; set; }
    }
}