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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Color>().HasIndex(c => c.Nombre).IsUnique();
        }
    }
}
