using api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.DataAccess.Configurators
{
    public class DataNotificationAPIConfigurator : IEntityTypeConfiguration<DataNotificationAPI>
    {
        public void Configure(EntityTypeBuilder<DataNotificationAPI> builder)
        {
            builder.ToTable("DataNotificationAPI");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id").HasColumnType("int");
            builder
                .Property(x => x.Response)
                .IsRequired()
                .HasColumnName("Response")
                .HasColumnType("nvarchar(MAX)");
        }
    }
}