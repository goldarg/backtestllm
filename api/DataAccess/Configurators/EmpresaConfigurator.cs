using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class EmpresaConfigurator : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("Empresas");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder.Property(x => x.razonSocial).IsRequired().HasColumnName("razonSocial").HasColumnType("nvarchar").HasMaxLength(150);
            builder.Property(x => x.guid).IsRequired().HasColumnName("guid").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.idCRM).IsRequired().HasColumnName("idCRM").HasColumnType("nvarchar").HasMaxLength(100);

            builder.HasMany(e => e.Asignaciones).WithOne(ue => ue.Empresa).HasForeignKey(q => q.empresaId);
        }
    }
}