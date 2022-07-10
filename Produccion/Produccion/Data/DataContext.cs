using Microsoft.EntityFrameworkCore;
using Produccion.Data.Entities;

namespace Produccion.Data
{
    public class DataContext : DbContext
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


    }
}
