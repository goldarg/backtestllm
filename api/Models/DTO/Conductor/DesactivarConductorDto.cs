using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.DTO.Vehiculo;

namespace api.Models.DTO.Conductor
{
    public class DesactivarConductorDto
    {
        public string? usuarioCrmId { get; set; }
        public List<VehiculoRelacionadoDto> vehiculosRelacionados { get; set; }
    }
}
