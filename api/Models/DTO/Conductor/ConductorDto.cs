using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;

namespace api.Models.DTO.Conductor
{
    public class ConductorDto
    {
        public string id { get; set; }
        public string? Full_Name { get; set; }
        //TODO scoring (falta conexion a GeoTab por parte de RDA)
        public IList<CRMRelatedObject> Empresas { get; set; }
        public string? Cargo { get; set; } //puesto
        public IList<string> Roles { get; set; }
        //TODO campo permiso (quedo pendiente ver con funcional si esto es el tema de Aprobador)
        public IList<CRMRelatedObject> VehiculosAsignados { get; set; }
        public string? Estado { get; set; } //Este campo viene de 
    }
}