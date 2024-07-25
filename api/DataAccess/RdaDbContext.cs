using System.Reflection;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.DataAccess;

public partial class RdaDbContext : DbContext
{
    public RdaDbContext(DbContextOptions<RdaDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    public RdaDbContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        LoadConfigurations(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    private void LoadConfigurations(ModelBuilder builder)
    {
        var currentAssembly = Assembly.GetAssembly(GetType());

        if (currentAssembly == null)
        {
            throw new Exception("No se pudo obtener el assembly");
        }

        builder.ApplyConfigurationsFromAssembly(currentAssembly);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
