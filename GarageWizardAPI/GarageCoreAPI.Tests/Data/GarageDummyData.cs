using GarageCoreMVC.Models;

namespace GarageCoreAPI.Tests.Data
{
    public static class GarageDummyData
    {
        // Vehicles
        public static readonly Vehicle[] Vehicles = {
                new Vehicle() { Name = "Dummy", IsOnRepair = true, RegID = "DU101", Vendor = "Dumbo", Wheels = 4 },
                new Vehicle() { Name = "Bummy", IsOnRepair = false, RegID = "DU102", Vendor = "Bumbo", Wheels = 4 }
        };
        public static readonly Vehicle NewVehicle = new Vehicle() { Name = "Rummy", IsOnRepair = false, RegID = "DU103", Vendor = "Rumbo", Wheels = 4 };
        public const string FindableVehicleID = "DU101";
        public const string NotFindableVehicleID = "XU101";
        // Spots
        public static readonly Spot[] Spots = {
                new Spot() { ID = "D101", Capacity = 4 ,Occupied = true },
                new Spot() { ID = "D102", Capacity = 2 ,Occupied = true },
                new Spot() { ID = "D103", Capacity = 8 ,Occupied = false }
        };
        public static readonly Spot NewSpot = new Spot() { ID = "R101", Capacity = 4, Occupied = false };
        public const string FindableSpotID = "D101";
        public const string NotFindableSpotID = "X101";
        // Parkings
        public static readonly ParkingTransaction[] Parkings = {
                new ParkingTransaction(){ ID = "p1" ,SpotID = "D101", VehicleID = "DU101", ParkStart = DateTime.Now.AddHours(-2), Service = GarageCoreMVC.Common.Enums.ServiceT.Economy , spot = null, vehicle = null, },
                new ParkingTransaction(){ ID = "p2" ,SpotID = "D102", VehicleID = "DU102", ParkStart = DateTime.Now.AddHours(-1), Service = GarageCoreMVC.Common.Enums.ServiceT.VIP , spot = null, vehicle = null, }
        };
        // Jobs
        public static readonly Job[] Jobs = {
                new Job() { ID = "j1", Name = "Job1", Description = "Good job !", Hours = 10, Charge = 100, JobImagePath = "path/to/luck"}
        };
        public static readonly Job NewJob = new Job() { Name = "Job2", Description = "Good job !", Hours = 5, Charge = 200, JobImagePath = "path/to/success" };
        public const string FindableJobID = "j1";
        public const string NotFindableJobID = "x1";
        // Repairs
        public static readonly RepairTransaction[] Repairs = {
                //new RepairTransaction(){ ID = "r1" , JobID = "j1", VehicleID = "DU101", job = null, vehicle = null, IsCompleted = false },
        };
        // Roles
        public static readonly Role[] Roles =
        {
            new Role { Name = "Sudo", Description = "I can do anything !"},
            new Role { Name = "Budo", Description = "I can't do anything !"}
        };
    }
}