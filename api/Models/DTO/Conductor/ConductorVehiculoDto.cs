using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Conductor
{
    public class ConductorVehiculoDto
    {
        public string? conductorCrmId { get; set; }
        public CRMRelatedObject? vehiculo { get; set; }
    }
}