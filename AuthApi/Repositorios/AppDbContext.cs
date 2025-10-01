using AuthApi.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using AuthApi.Entidades.AuthApi.Entidades;

// Nota: He quitado el using duplicado y el innecesario para limpiar.

namespace AuthApi.Repositorios
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔑 CORRECCIÓN CRÍTICA: Forzar a EF Core a usar los nombres de tablas
            // en minúsculas y plural que usa MySQL. Esto soluciona el error de clave foránea.
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Rol>().ToTable("roles");
            modelBuilder.Entity<Producto>().ToTable("productos");
            modelBuilder.Entity<Categoria>().ToTable("categorias");

            // Relación Usuario <-> Rol (ForeignKey RolId)
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            // Relación Producto <-> Categoria
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId);
        }
    }
}