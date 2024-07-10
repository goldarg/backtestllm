using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class RolesConfigurator
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.id);

            builder.Property(x => x.id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder.Property(x => x.nombreRol).IsRequired().HasColumnName("razonSocial").HasColumnType("nvarchar").HasMaxLength(150);
            builder.Property(x => x.guid).IsRequired().HasColumnName("guid").HasColumnType("uniqueidentifier").IsRequired();
        }
    }
}