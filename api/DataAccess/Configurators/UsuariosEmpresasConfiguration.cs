using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class UsuariosEmpresasConfiguration
    {
        public void Configure(EntityTypeBuilder<UsuariosEmpresas> builder)
        {
            builder.ToTable("UsuariosEmpresas");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder.Property(x => x.userId).IsRequired().HasColumnName("userId").HasColumnType("int");
            builder.Property(x => x.empresaId).IsRequired().HasColumnName("empresaId").HasColumnType("int");

            builder.HasOne(x => x.User).WithMany(q => q.EmpresasAsignaciones).HasForeignKey(w => w.userId).OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(x => x.Empresa).WithMany(q => q.Asignaciones).HasForeignKey(w => w.empresaId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}