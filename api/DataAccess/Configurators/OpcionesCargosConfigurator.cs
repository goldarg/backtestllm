using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators;

public class OpcionesCargosConfigurator : IEntityTypeConfiguration<OpcionesCargos>
{
    public void Configure(EntityTypeBuilder<OpcionesCargos> builder)
    {
        builder.HasKey(e => e.Nombre);
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
    }
}
