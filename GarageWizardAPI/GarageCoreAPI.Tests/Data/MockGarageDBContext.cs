using GarageCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GarageCoreAPI.Tests.Data
{
    public class MockGarageDBContext: GarageDBContext
    {
        public MockGarageDBContext(IConfiguration config) : base(config)
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase($"GarageTestDB_{DateTime.Now.Ticks}");
        }

        public void SeedDatabaseContext()
        {
            if (Vehicles.Any() || Spots.Any() || Parkings.Any() || Jobs.Any())
            {
                return; // Avoid seeding if data already exists
            }
            foreach (var vehicle in GarageDummyData.Vehicles)
            {
                this.Vehicles.Add(vehicle);
            }
            foreach (var spot in GarageDummyData.Spots)
            {
                this.Spots.Add(spot);
            }
            foreach (var parking in GarageDummyData.Parkings)
            {
                this.Parkings.Add(parking);
            }
            foreach (var job in GarageDummyData.Jobs)
            {
                this.Jobs.Add(job);
            }
            this.SaveChanges();
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }

        public override void Dispose()
        {
            if(this.Database.IsInMemory())
            {
                this.Database.EnsureDeleted();
            }
            base.Dispose();
        }
    }
}
