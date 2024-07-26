using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.DTO.Vehiculo;

namespace api.Models.DTO.Conductor
{
    public class ConductorVehiculoDto
    {
        public string? conductorCrmId { get; set; }
        public VehiculoRelacionadoDto? vehiculo { get; set; }
    }
}
