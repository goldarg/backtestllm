using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");

            builder
                .Property(x => x.nombreCompleto)
                .IsRequired()
                .HasColumnName("nombreCompleto")
                .HasColumnType("nvarchar")
                .HasMaxLength(300);

            builder
                .Property(x => x.email)
                .IsRequired()
                .HasColumnName("email")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder
                .Property(x => x.telefono)
                .IsRequired()
                .HasColumnName("telefono")
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.empresaId)
                .IsRequired()
                .HasColumnName("empresaId")
                .HasColumnType("int");

            builder
                .Property(x => x.dominio)
                .IsRequired()
                .HasColumnName("dominio")
                .HasColumnType("nvarchar")
                .HasMaxLength(20);

            builder
                .Property(x => x.departamento)
                .IsRequired()
                .HasColumnName("departamento")
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.tipoOperacion)
                .IsRequired()
                .HasColumnName("tipoOperacion")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder
                .Property(x => x.asunto)
                .IsRequired()
                .HasColumnName("asunto")
                .HasColumnType("nvarchar")
                .HasMaxLength(200);

            builder
                .Property(x => x.zona)
                .IsRequired()
                .HasColumnName("zona")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            builder
                .Property(x => x.descripcion)
                .IsRequired()
                .HasColumnName("descripcion")
                .HasColumnType("nvarchar(max)");

            builder
                .Property(x => x.odometro)
                .IsRequired()
                .HasColumnName("odometro")
                .HasColumnType("int");

            builder
                .Property(x => x.turnoOpcion1)
                .IsRequired()
                .HasColumnName("turnoOpcion1")
                .HasColumnType("datetime");

            builder
                .Property(x => x.turnoOpcion2)
                .IsRequired()
                .HasColumnName("turnoOpcion2")
                .HasColumnType("datetime");

            builder
                .Property(x => x.idTiquetera)
                .IsRequired()
                .HasColumnName("idTiquetera")
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.numeroTicket)
                .IsRequired()
                .HasColumnName("numeroTicket")
                .HasColumnType("nvarchar")
                .HasMaxLength(50);

            builder
                .Property(x => x.solicitanteId)
                .IsRequired()
                .HasColumnName("solicitanteId")
                .HasColumnType("int");

            builder
                .HasOne(x => x.Empresa)
                .WithMany()
                .HasForeignKey(z => z.empresaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Solicitante)
                .WithMany()
                .HasForeignKey(z => z.solicitanteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
