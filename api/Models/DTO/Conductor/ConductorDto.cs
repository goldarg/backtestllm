using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.DTO.Empresa;
using api.Models.DTO.Rol;
using api.Models.Entities;

namespace api.Models.DTO.Conductor
{
    public class ConductorDto
    {
        public string id { get; set; }
        public string? Full_Name { get; set; }
        //TODO scoring (falta conexion a GeoTab por parte de RDA)
        public IList<EmpresaDto> Empresas { get; set; }
        public string? Cargo { get; set; } //puesto
        public IList<RolDto> Roles { get; set; }
        //TODO campo permiso (quedo pendiente ver con funcional si esto es el tema de Aprobador)
        public IList<CRMRelatedObject> VehiculosAsignados { get; set; } = new List<CRMRelatedObject>();
        public string? Estado { get; set; } //Este campo viene de 
    }
}