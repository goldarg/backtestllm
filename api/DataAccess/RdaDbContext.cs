using Microsoft.EntityFrameworkCore;

namespace api.DataAccess;

public class RdaDbContext : DbContext
{
    public RdaDbContext(DbContextOptions<RdaDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }
}