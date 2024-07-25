using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class UsuarioEstadoConfiguration
    {
        public void Configure(EntityTypeBuilder<UsuarioEstado> builder)
        {
            builder.ToTable("UsuarioEstados");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder
                .Property(x => x.descripcion)
                .IsRequired()
                .HasColumnName("descripcion")
                .HasColumnType("nvarchar")
                .HasMaxLength(200);
        }
    }
}
