using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class ActividadUsuarioConfigurator : IEntityTypeConfiguration<ActividadUsuario>
    {
        public void Configure(EntityTypeBuilder<ActividadUsuario> builder)
        {
            builder.ToTable("ActividadUsuarios");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");

            builder
                .Property(x => x.descripcion)
                .IsRequired()
                .HasColumnName("descripcion")
                .HasColumnType("nvarchar")
                .HasMaxLength(500);

            builder
                .Property(x => x.fecha)
                .IsRequired()
                .HasColumnName("fecha")
                .HasColumnType("datetime");

            builder
                .Property(x => x.usuarioAfectadoId)
                .IsRequired()
                .HasColumnName("usuarioAfectadoId")
                .HasColumnType("int");

            builder
                .Property(x => x.usuarioEjecutorId)
                .IsRequired()
                .HasColumnName("usuarioEjecutorId")
                .HasColumnType("int");

            //Relaciones
            builder
                .HasOne(x => x.usuarioAfectado)
                .WithMany()
                .HasForeignKey(q => q.usuarioAfectadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.usuarioEjecutor)
                .WithMany()
                .HasForeignKey(z => z.usuarioEjecutorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
