﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using api.DataAccess;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(RdaDbContext))]
    [Migration("20240816204722_UserTelefono")]
    partial class UserTelefono
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("api.Models.Entities.ActividadUsuario", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar")
                        .HasColumnName("descripcion");

                    b.Property<DateTime>("fecha")
                        .HasColumnType("datetime")
                        .HasColumnName("fecha");

                    b.Property<int>("usuarioAfectadoId")
                        .HasColumnType("int")
                        .HasColumnName("usuarioAfectadoId");

                    b.Property<int>("usuarioEjecutorId")
                        .HasColumnType("int")
                        .HasColumnName("usuarioEjecutorId");

                    b.HasKey("id");

                    b.HasIndex("usuarioAfectadoId");

                    b.HasIndex("usuarioEjecutorId");

                    b.ToTable("ActividadUsuarios", (string)null);
                });

            modelBuilder.Entity("api.Models.Entities.DataNotificationAPI", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasColumnType("nvarchar(MAX)")
                        .HasColumnName("Response");

                    b.HasKey("Id");

                    b.ToTable("DataNotificationAPI", (string)null);
                });

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

            modelBuilder.Entity("api.Models.Entities.OpcionesCargos", b =>
                {
                    b.Property<string>("Nombre")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Nombre");

                    b.ToTable("OpcionesCargos");
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

            modelBuilder.Entity("api.Models.Entities.Ticket", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("asunto")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar")
                        .HasColumnName("asunto");

                    b.Property<string>("departamento")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar")
                        .HasColumnName("departamento");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("descripcion");

                    b.Property<string>("dominio")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar")
                        .HasColumnName("dominio");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar")
                        .HasColumnName("email");

                    b.Property<int>("empresaId")
                        .HasColumnType("int")
                        .HasColumnName("empresaId");

                    b.Property<string>("idTiquetera")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar")
                        .HasColumnName("idTiquetera");

                    b.Property<string>("nombreCompleto")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar")
                        .HasColumnName("nombreCompleto");

                    b.Property<string>("numeroTicket")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar")
                        .HasColumnName("numeroTicket");

                    b.Property<int>("odometro")
                        .HasColumnType("int")
                        .HasColumnName("odometro");

                    b.Property<int>("solicitanteId")
                        .HasColumnType("int")
                        .HasColumnName("solicitanteId");

                    b.Property<string>("telefono")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar")
                        .HasColumnName("telefono");

                    b.Property<string>("tipoOperacion")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar")
                        .HasColumnName("tipoOperacion");

                    b.Property<DateTime>("turnoOpcion1")
                        .HasColumnType("datetime")
                        .HasColumnName("turnoOpcion1");

                    b.Property<DateTime>("turnoOpcion2")
                        .HasColumnType("datetime")
                        .HasColumnName("turnoOpcion2");

                    b.Property<string>("zona")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar")
                        .HasColumnName("zona");

                    b.HasKey("id");

                    b.HasIndex("empresaId");

                    b.HasIndex("solicitanteId");

                    b.ToTable("Tickets", (string)null);
                });

            modelBuilder.Entity("api.Models.Entities.User", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("apellido")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("estado")
                        .HasColumnType("nvarchar(max)");

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

                    b.ToTable("User");
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

            modelBuilder.Entity("api.Models.Entities.ActividadUsuario", b =>
                {
                    b.HasOne("api.Models.Entities.User", "usuarioAfectado")
                        .WithMany()
                        .HasForeignKey("usuarioAfectadoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("api.Models.Entities.User", "usuarioEjecutor")
                        .WithMany()
                        .HasForeignKey("usuarioEjecutorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("usuarioAfectado");

                    b.Navigation("usuarioEjecutor");
                });

            modelBuilder.Entity("api.Models.Entities.Ticket", b =>
                {
                    b.HasOne("api.Models.Entities.Empresa", "Empresa")
                        .WithMany()
                        .HasForeignKey("empresaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("api.Models.Entities.User", "Solicitante")
                        .WithMany()
                        .HasForeignKey("solicitanteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Empresa");

                    b.Navigation("Solicitante");
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
