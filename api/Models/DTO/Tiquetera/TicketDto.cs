using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.Tiquetera
{
    public class TicketDto
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? subject { get; set; }
        public string? status { get; set; }
    }
}
