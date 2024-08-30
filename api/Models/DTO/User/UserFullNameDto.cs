using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTO.User
{
    public class UserSummaryDto
    {
        public string? id { get; set; }
        public string? email { get; set; }
        public string? Full_Name { get; set; }
    }
}
