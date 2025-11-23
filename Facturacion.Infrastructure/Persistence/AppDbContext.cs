using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Facturacion.Domain.Entities;

namespace Facturacion.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      // TABLAS DE TU BASE DE DATOS
      public DbSet<Usuario> Usuarios => Set<Usuario>();
      public DbSet<Cliente> Clientes => Set<Cliente>();
      public DbSet<Producto> Productos => Set<Producto>();
      public DbSet<ProductoLote> ProductoLotes => Set<ProductoLote>();  // ¡ESTO FALTABA!
                                                                        // Dentro de la clase AppDbContext agrega esto junto a los otros DbSet:
      public DbSet<CategoriaProducto> CategoriaProductos => Set<CategoriaProducto>();
      public DbSet<Marca> Marca { get; set; }
      public DbSet<CategoriaProducto> Categorias { get; set; }
      public DbSet<Factura> Facturas { get; set; }
      public DbSet<FacturaDetalle> FacturaDetalles { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);

            // ======================
            // CONFIGURACIÓN CLIENTES
            // ======================
            modelBuilder.Entity<Cliente>(entity =>
            {
                  entity.HasKey(c => c.Id);

                  entity.Property(c => c.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();
                  ;


                  entity.Property(c => c.TipoIdentificacion)
                    .HasMaxLength(20)
                    .IsRequired();

                  entity.Property(c => c.Identificacion)
                    .HasMaxLength(20)
                    .IsRequired();

                  // Índice único: no puede haber dos clientes activos con mismo tipo + número
                  entity.HasIndex(c => new { c.TipoIdentificacion, c.Identificacion })
                    .IsUnique()
                    .HasFilter("[Activo] = 1");

                  entity.Property(c => c.Email).HasMaxLength(100);
                  entity.Property(c => c.Telefono).HasMaxLength(20);
                  entity.Property(c => c.Direccion).HasMaxLength(200);
            });

            // ======================
            // CONFIGURACIÓN PRODUCTOS
            // ======================
            // En la configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                  entity.HasKey(p => p.Id);

                  entity.Property(p => p.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                  entity.Property(p => p.Codigo).HasMaxLength(50);
                  entity.Property(p => p.CodigoBarra).HasMaxLength(50);

                  // CAMBIO AQUÍ: ya no son string, son FK
                  entity.Property(p => p.MarcaId);      // opcional si es nullable
                  entity.Property(p => p.CategoriaId);

                  entity.Property(p => p.Modelo).HasMaxLength(100); // ← este SÍ sigue siendo string

                  entity.HasOne(p => p.Marca)
                    .WithMany()
                    .HasForeignKey(p => p.MarcaId)
                    .OnDelete(DeleteBehavior.SetNull);

                  entity.HasOne(p => p.Categoria)
                    .WithMany()
                    .HasForeignKey(p => p.CategoriaId)
                    .OnDelete(DeleteBehavior.SetNull);

                  entity.HasMany(p => p.Lotes)
                    .WithOne(l => l.Producto)
                    .HasForeignKey(l => l.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================
            // CONFIGURACIÓN PRODUCTO LOTES
            // ======================
            modelBuilder.Entity<ProductoLote>(entity =>
            {
                  entity.HasKey(pl => pl.Id);

                  entity.Property(pl => pl.Lote)
                    .HasMaxLength(50)
                    .IsRequired();

                  entity.HasIndex(pl => new { pl.ProductoId, pl.Lote })
                    .IsUnique();

                  entity.Property(pl => pl.PrecioCompra)
                    .HasColumnType("decimal(18,2)");

                  entity.Property(pl => pl.PrecioVenta)
                    .HasColumnType("decimal(18,2)");
            });

            // ======================
            // RELACIÓN CON USUARIO CREADOR (opcional pero recomendado)
            // ======================
            modelBuilder.Entity<Cliente>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductoLote>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(pl => pl.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // ======================
            // RELACIÓN CON Factura Y FACTURA DETALLE
            // ======================
            
            modelBuilder.Entity<Factura>()
                .HasMany(f => f.Detalles)
                .WithOne(d => d.Factura)
                .HasForeignKey(d => d.FacturaId);

      }

}