using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO
{
    public class BuscarConductorResponse
    {
        public List<BuscarConductorDto> Data { get; set; }
    }

    public class BuscarConductorDto
    {
        public string id { get; set; }
        public Conductor Conductor { get; set; }
    }
}