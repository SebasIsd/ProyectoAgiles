using Microsoft.EntityFrameworkCore;
using Facturacion.Domain.Entities;

namespace Facturacion.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductoLote> ProductoLotes => Set<ProductoLote>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<CategoriaProducto> CategoriaProductos => Set<CategoriaProducto>();
    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<FacturaDetalle> FacturaDetalles => Set<FacturaDetalle>();
    public DbSet<Secuencial> Secuenciales => Set<Secuencial>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ CONFIGURACIÓN MARCA
        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
        });

        // ✅ CONFIGURACIÓN CATEGORÍA
        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
        });

        // ✅ CONFIGURACIÓN PRODUCTO - AMBAS RELACIONES CON NAVEGACIÓN INVERSA
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Codigo).HasMaxLength(50);
            entity.Property(p => p.CodigoBarra).HasMaxLength(50);
            entity.Property(p => p.Modelo).HasMaxLength(100);

            // ✅ RELACIÓN CON MARCA - CON NAVEGACIÓN INVERSA
            entity.HasOne(p => p.Marca)
                  .WithMany(m => m.Productos)
                  .HasForeignKey(p => p.MarcaId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);

            // ✅ RELACIÓN CON CATEGORÍA - CON NAVEGACIÓN INVERSA (¡ESTE ERA EL PROBLEMA!)
            entity.HasOne(p => p.Categoria)
                  .WithMany(c => c.Productos)  // ← CAMBIO CRÍTICO: usa la navegación inversa
                  .HasForeignKey(p => p.CategoriaId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductoLote>(entity =>
        {
            entity.HasKey(pl => pl.Id);
            entity.Property(pl => pl.Lote).HasMaxLength(50).IsRequired();
            entity.Property(pl => pl.PrecioCompra).HasPrecision(18, 2);
            entity.Property(pl => pl.PrecioVenta).HasPrecision(18, 2);

            entity.HasOne(pl => pl.Producto)
                  .WithMany(p => p.Lotes)
                  .HasForeignKey(pl => pl.ProductoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ✅ CONFIGURACIÓN PARA DECIMALES
        modelBuilder.Entity<Factura>(entity =>
        {
            entity.Property(f => f.Iva).HasPrecision(18, 2);
            entity.Property(f => f.Subtotal).HasPrecision(18, 2);
            entity.Property(f => f.Total).HasPrecision(18, 2);
            entity.Property(f => f.TotalDescuento).HasPrecision(18, 2);
        });

        modelBuilder.Entity<FacturaDetalle>(entity =>
        {
            entity.Property(fd => fd.PorcentajeIva).HasPrecision(5, 2);
            entity.Property(fd => fd.PrecioUnitario).HasPrecision(18, 2);
            entity.Property(fd => fd.SubtotalLinea).HasPrecision(18, 2);
            entity.Property(fd => fd.TotalLinea).HasPrecision(18, 2);
            entity.Property(fd => fd.ValorIva).HasPrecision(18, 2);
        });

        // ✅ LIMPIAR SHADOW PROPERTIES (OPCIONAL PERO RECOMENDADO)
        var productoEntity = modelBuilder.Entity<Producto>();
        productoEntity.Ignore("CategoriaProductoId");
        productoEntity.Ignore("CategoriaProductoId1");
        productoEntity.Ignore("CategoriaProductoId2");
        productoEntity.Ignore("CategoriaProductoId3");
        productoEntity.Ignore("CategoriaProductoId4");
        productoEntity.Ignore("MarcaId1");
        productoEntity.Ignore("MarcaId2");
        productoEntity.Ignore("MarcaId3");
        productoEntity.Ignore("MarcaId4");
    }
}