using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Produccion.Data.Entities;

namespace Produccion.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Color> Colors { get; set; }
        public DbSet<Fabric> Fabrics { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<Garment> Garments { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Color>().HasIndex(c => c.Nombre).IsUnique();
            modelBuilder.Entity<Fabric>().HasIndex(c => c.Nombre).IsUnique();
            modelBuilder.Entity<Garment>().HasIndex(c => c.Nombre).IsUnique();
        }

    }
}
