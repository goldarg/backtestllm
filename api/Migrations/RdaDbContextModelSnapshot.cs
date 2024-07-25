﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using api.DataAccess;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(RdaDbContext))]
    partial class RdaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Entities.Empresa", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<Guid>("guid")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("guid");

                    b.Property<string>("idCRM")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar")
                        .HasColumnName("idCRM");

                    b.Property<string>("razonSocial")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar")
                        .HasColumnName("razonSocial");

                    b.HasKey("id");

                    b.ToTable("Empresas", (string)null);
                });

            modelBuilder.Entity("api.Models.Entities.Rol", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<Guid>("guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("jerarquia")
                        .HasColumnType("int");

                    b.Property<string>("nombreRol")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("Rol");
                });

            modelBuilder.Entity("api.Models.Entities.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("UsuarioEstadoid")
                        .HasColumnType("int");

                    b.Property<string>("apellido")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("estadoId")
                        .HasColumnType("int");

                    b.Property<Guid>("guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("idCRM")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("isRDA")
                        .HasColumnType("bit");

                    b.Property<string>("nombre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("telefono")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("userName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.HasIndex("UsuarioEstadoid");

                    b.ToTable("User");
                });

            modelBuilder.Entity("api.Models.Entities.UsuarioEstado", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("UsuarioEstado");
                });

            modelBuilder.Entity("api.Models.Entities.UsuariosEmpresas", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("empresaId")
                        .HasColumnType("int");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("empresaId");

                    b.HasIndex("userId");

                    b.ToTable("UsuariosEmpresas");
                });

            modelBuilder.Entity("api.Models.Entities.UsuariosRoles", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("rolId")
                        .HasColumnType("int");

                    b.Property<int>("userId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("rolId");

                    b.HasIndex("userId");

                    b.ToTable("UsuariosRoles");
                });

            modelBuilder.Entity("api.Models.Entities.User", b =>
                {
                    b.HasOne("api.Models.Entities.UsuarioEstado", "UsuarioEstado")
                        .WithMany()
                        .HasForeignKey("UsuarioEstadoid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UsuarioEstado");
                });

            modelBuilder.Entity("api.Models.Entities.UsuariosEmpresas", b =>
                {
                    b.HasOne("api.Models.Entities.Empresa", "Empresa")
                        .WithMany("Asignaciones")
                        .HasForeignKey("empresaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.Entities.User", "User")
                        .WithMany("EmpresasAsignaciones")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Empresa");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Entities.UsuariosRoles", b =>
                {
                    b.HasOne("api.Models.Entities.Rol", "Rol")
                        .WithMany("Asignaciones")
                        .HasForeignKey("rolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("api.Models.Entities.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("api.Models.Entities.Empresa", b =>
                {
                    b.Navigation("Asignaciones");
                });

            modelBuilder.Entity("api.Models.Entities.Rol", b =>
                {
                    b.Navigation("Asignaciones");
                });

            modelBuilder.Entity("api.Models.Entities.User", b =>
                {
                    b.Navigation("EmpresasAsignaciones");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
