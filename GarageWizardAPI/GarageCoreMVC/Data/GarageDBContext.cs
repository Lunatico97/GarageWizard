using GarageCoreMVC.Common.Configurations;
using GarageCoreMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GarageCoreMVC.Data
{
    public class GarageDBContext : IdentityDbContext<User, Role, string>
    {
        protected readonly IConfiguration _config;

        public GarageDBContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_config.GetConnectionString(GlobalConfig.ConnectionStringPSQL));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<ParkingTransaction> Parkings { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<RepairTransaction> Repairs { get; set; }
    }
}
