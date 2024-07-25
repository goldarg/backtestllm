using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Contrato
{
    public class ContratoResponse
    {
        public string? Name { get; set; }
        public string? id { get; set; }
        public CRMRelatedObject Cuenta { get; set; }
    }
}
