using ApiUsuariosRC.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiUsuariosRC.DBOperation.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Genero> Generos => Set<Genero>();

    public DbSet<UsuarioCambioLog> UsuarioCambiosLog => Set<UsuarioCambioLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var usuario = modelBuilder.Entity<Usuario>();
        usuario.ToTable("Usuarios");
        usuario.HasKey(x => x.UsuarioId);
        usuario.Property(x => x.UsuarioId).ValueGeneratedOnAdd();
        usuario.Property(x => x.NumeroEmpleado).IsRequired().HasMaxLength(50);
        usuario.Property(x => x.GeneroId).IsRequired();
        usuario.Property(x => x.Nombres).IsRequired().HasMaxLength(150);
        usuario.Property(x => x.ApellidoPaterno).IsRequired().HasMaxLength(100);
        usuario.Property(x => x.ApellidoMaterno).IsRequired().HasMaxLength(100);
        usuario.Property(x => x.Correo).IsRequired().HasMaxLength(150);
        usuario.Property(x => x.Activo).HasDefaultValue(true);
        usuario.Property(x => x.FechaNacimiento).IsRequired().HasColumnType("datetime2(0)");
        usuario.Property(x => x.FechaCreacion).IsRequired().HasColumnType("datetime2(0)");
        usuario.Property(x => x.FechaActualizacion).HasColumnType("datetime2(0)");
        usuario.HasIndex(x => x.NumeroEmpleado).IsUnique();
        usuario.HasIndex(x => x.Correo).IsUnique();
        usuario.HasIndex(x => x.GeneroId);
        usuario.HasOne(x => x.Genero)
            .WithMany(x => x.Usuarios)
            .HasForeignKey(x => x.GeneroId)
            .OnDelete(DeleteBehavior.Restrict);

        var genero = modelBuilder.Entity<Genero>();
        genero.ToTable("Genero");
        genero.HasKey(x => x.GeneroId);
        genero.Property(x => x.GeneroId).ValueGeneratedOnAdd();
        genero.Property(x => x.Nombre).HasColumnName("Genero").IsRequired().HasMaxLength(100);
        genero.Property(x => x.FechaCreacion).IsRequired().HasColumnType("datetime2(0)");
        genero.HasIndex(x => x.Nombre).IsUnique();

        var usuarioConsulta = modelBuilder.Entity<UsuarioConsulta>();
        usuarioConsulta.HasNoKey();
        usuarioConsulta.ToView(null);

        var log = modelBuilder.Entity<UsuarioCambioLog>();
        log.ToTable("UsuarioCambiosLog");
        log.HasKey(x => x.LogId);
        log.Property(x => x.LogId).ValueGeneratedOnAdd();
        log.Property(x => x.UsuarioIdAfectado).IsRequired();
        log.Property(x => x.UsuarioIdAccion).IsRequired();
        log.Property(x => x.TipoOperacion).IsRequired().HasMaxLength(50);
        log.Property(x => x.CampoModificado).IsRequired().HasMaxLength(100);
        log.Property(x => x.ValorAnterior);
        log.Property(x => x.ValorNuevo);
        log.Property(x => x.FechaOperacion).IsRequired().HasColumnType("datetimeoffset(0)");
        log.HasIndex(x => x.UsuarioIdAfectado);
        log.HasIndex(x => x.UsuarioIdAccion);
    }
}
