using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators;

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
            .HasColumnType("nvarchar")
            .HasMaxLength(100);
        builder
            .Property(x => x.apellido)
            .IsRequired()
            .HasColumnName("apellido")
            .HasColumnType("nvarchar")
            .HasMaxLength(100);
        builder
            .Property(x => x.telefono)
            .IsRequired()
            .HasColumnName("telefono")
            .HasColumnType("nvarchar")
            .HasMaxLength(100);
        builder
            .Property(x => x.estado)
            .IsRequired()
            .HasColumnName("estado")
            .HasColumnType("nvarchar")
            .HasMaxLength(200);
        builder
            .Property(x => x.guid)
            .IsRequired()
            .HasColumnName("guid")
            .HasColumnType("uniqueidentifier")
            .IsRequired();
        builder
            .Property(x => x.isRDA)
            .IsRequired()
            .HasColumnName("isRDA")
            .HasColumnType("bit")
            .HasDefaultValue(1);
        builder
            .Property(x => x.idCRM)
            .IsRequired()
            .HasColumnName("idCRM")
            .HasColumnType("nvarchar")
            .HasMaxLength(100);

        //Relaciones
        builder
            .HasMany(x => x.EmpresasAsignaciones)
            .WithOne(u => u.User)
            .HasForeignKey(q => q.userId);
    }
}
