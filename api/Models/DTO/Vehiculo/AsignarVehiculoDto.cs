using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Vehiculo
{
    public class AsignarVehiculoDto
    {
        public string? usuarioId { get; set; }
        public string tipoContrato { get; set; }
        public string? idContratoInterno { get; set; }
    }
}
