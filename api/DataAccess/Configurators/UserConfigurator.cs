using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace api.DataAccess.Configurators
{
    public class UserConfigurator
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder
                .Property(x => x.userName)
                .IsRequired()
                .HasColumnName("userName")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);
            builder
                .Property(x => x.nombre)
                .IsRequired()
                .HasColumnName("nombre")
                .HasColumnType("nvarhar")
                .HasMaxLength(100);
            builder
                .Property(x => x.apellido)
                .IsRequired()
                .HasColumnName("apellido")
                .HasColumnType("nvarhar")
                .HasMaxLength(100);
            builder
                .Property(x => x.estadoId)
                .IsRequired()
                .HasColumnName("estadoId")
                .HasColumnType("int")
                .HasDefaultValue(1);
            builder
                .Property(x => x.guid)
                .IsRequired()
                .HasColumnName("guid")
                .HasColumnType("uniqueidentifier")
                .IsRequired();
            builder
                .Property(x => x.isRDA)
                .IsRequired()
                .HasColumnName("guid")
                .HasColumnType("bit")
                .HasDefaultValue(1);
            builder
                .Property(x => x.idCRM)
                .IsRequired()
                .HasColumnName("idCRM")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);
            builder
                .Property(x => x.telefono)
                .HasColumnName("telefono")
                .HasColumnType("nvarchar")
                .HasMaxLength(100);

            //Relaciones
            builder
                .HasMany(x => x.EmpresasAsignaciones)
                .WithOne(u => u.User)
                .HasForeignKey(q => q.userId);

            builder
                .HasOne(u => u.UsuarioEstado)
                .WithMany()
                .HasForeignKey(u => u.estadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
