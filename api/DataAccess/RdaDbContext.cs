using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class RdaDbContext : DbContext
    {
        public RdaDbContext(DbContextOptions<RdaDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
    }
}