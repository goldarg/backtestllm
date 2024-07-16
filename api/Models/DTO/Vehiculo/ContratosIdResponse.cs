using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Vehiculo
{
    public class ContratosIdResponse
    {
        public List<ContratosIdDto> Data { get; set; }
    }

    public class ContratosIdDto
    {
        public int id { get; set; }
        public string Tipo_de_Contrato { get; set; }
    }
}