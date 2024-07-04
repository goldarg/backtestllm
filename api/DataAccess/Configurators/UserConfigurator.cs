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
        public void ConfigureEntity(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.id);
            
            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder.Property(x => x.userName).IsRequired().HasColumnName("userName").HasColumnType("nvarchar").HasMaxLength(100);
            builder.Property(x => x.nombre).IsRequired().HasColumnName("nombre").HasColumnType("nvarhar").HasMaxLength(100);
            builder.Property(x => x.apellido).IsRequired().HasColumnName("apellido").HasColumnType("nvarhar").HasMaxLength(100);
            builder.Property(x => x.activo).IsRequired().HasColumnName("activo").HasColumnType("bit").HasDefaultValue(1);
            builder.Property(x => x.empresaId).IsRequired().HasColumnName("empresaId").HasColumnType("int");
            builder.Property(x => x.guid).IsRequired().HasColumnName("guid").HasColumnType("uniqueidentifier").IsRequired();

            builder.HasOne(x => x.Empresa).WithMany(q => q.Usuarios).HasForeignKey(w => w.empresaId);
        }
    }
}