using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class UsuariosRolesConfiguration
    {
        public void Configure(EntityTypeBuilder<UsuariosRoles> builder)
        {
            builder.ToTable("UsuariosRoles");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder.Property(x => x.userId).IsRequired().HasColumnName("userId").HasColumnType("int");
            builder.Property(x => x.rolId).IsRequired().HasColumnName("rolId").HasColumnType("int");

            builder.HasOne(x => x.Rol).WithMany(w => w.Asignaciones).HasForeignKey(q => q.rolId).OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(x => x.User).WithMany(w => w.Roles).HasForeignKey(z => z.userId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}