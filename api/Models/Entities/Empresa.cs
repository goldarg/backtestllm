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
        public string? codigo { get; set; }
        public Guid guid { get; set;}
        public ICollection<User>? Usuarios { get; set; }
        public ICollection<UsuariosEmpresas>? Asignaciones { get; set; }
    }
}