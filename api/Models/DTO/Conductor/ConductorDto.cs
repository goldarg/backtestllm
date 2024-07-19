using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.DTO.Empresa;

namespace api.Models.DTO.Conductor
{
    public class ConductorDto
    {
        public int id { get; set; }
        public string? idCRM { get; set; }
        public string? userName { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public EmpresaDto? Empresa { get; set; }
    }
}