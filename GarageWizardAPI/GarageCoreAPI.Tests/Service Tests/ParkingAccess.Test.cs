using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GarageCoreAPI.Tests.Services
{
    public class ParkingAccessTest : IDisposable
    {
        private readonly MockGarageDBContext _mockContext;
        private readonly ParkingAccess _parkingAccess;

        public ParkingAccessTest()
        {
            _mockContext = new MockGarageDBContext(null);
            _mockContext.SeedDatabaseContext();
            _parkingAccess = new ParkingAccess(_mockContext);
        }

        [Fact]
        public async Task GetParkingTransactions_ReturnsAllParkings()
        {
            // Act
            var result = await _parkingAccess.GetParkingTransactions();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<ParkingTransaction>>(result);
        }

        [Fact]
        public async Task AddParking_SuccesssfulInsertion()
        {
            // Arrange
            var newParking = new ParkingTransaction() { SpotID = GarageDummyData.FindableSpotID, VehicleID = GarageDummyData.FindableVehicleID, Service = ServiceT.Economy };

            // Act
            await _parkingAccess.AddParking(newParking.SpotID, newParking.VehicleID, newParking.Service);

            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == newParking.SpotID && parking.VehicleID == newParking.VehicleID);
            Assert.NotNull(parking);
            Assert.IsType<ParkingTransaction>(parking);
        }

        [Fact]
        public async Task CheckoutParking_UpdatesParkEndTime_WhenParkingFound()
        {
            // Arrange
            var newParking = new ParkingTransaction() { SpotID = GarageDummyData.FindableSpotID, VehicleID = GarageDummyData.FindableVehicleID, Service = ServiceT.Economy };
            // Act
            await _parkingAccess.CheckoutParking(newParking.SpotID, newParking.VehicleID);
            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == newParking.SpotID && parking.VehicleID == newParking.VehicleID);
            Assert.NotNull(parking);
            Assert.IsType<ParkingTransaction>(parking);
            Assert.NotEqual(DateTime.MaxValue, parking.ParkEnd);
        }

        [Fact]
        public async Task RemoveParking_SuccessfulRemoval_WhenParkingFound()
        {
            // Arrange
            string spotID = GarageDummyData.FindableSpotID;
            string vehicleID = GarageDummyData.FindableVehicleID;
            // Act
            await _parkingAccess.RemoveParking(spotID, vehicleID);
            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == spotID && parking.VehicleID == vehicleID);
            Assert.Null(parking);
        }

        [Fact]
        public async Task RemoveParking_SuccessfulRemoval_WhenParkingNotFound()
        {
            // Arrange
            string spotID = GarageDummyData.NotFindableSpotID;
            string vehicleID = GarageDummyData.NotFindableVehicleID;
            // Act
            await _parkingAccess.RemoveParking(spotID, vehicleID);
            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == spotID && parking.VehicleID == vehicleID);
            Assert.Null(parking);
        }

        [Fact]
        public async Task GetParkingByID_ReturnsParkingTransaction_WhenParkingFound()
        {
            // Arrange
            string spotID = GarageDummyData.FindableSpotID;
            string vehicleID = GarageDummyData.FindableVehicleID;
            // Act
            await _parkingAccess.GetParkingByID(spotID, vehicleID);
            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == spotID && parking.VehicleID == vehicleID);
            Assert.IsType<ParkingTransaction>(parking);
            Assert.NotNull(parking);
        }

        [Fact]
        public async Task GetParkingByID_ReturnsParkingTransaction_WhenParkingNotFound()
        {
            // Arrange
            string spotID = GarageDummyData.NotFindableSpotID;
            string vehicleID = GarageDummyData.NotFindableVehicleID;
            // Act
            await _parkingAccess.GetParkingByID(spotID, vehicleID);
            // Assert
            var parking = await _mockContext.Parkings.FirstOrDefaultAsync(parking => parking.SpotID == spotID && parking.VehicleID == vehicleID);
            Assert.Null(parking);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(2, 10)]
        public void CalculateParkedTime_ReturnsCorrectTimeSpan(double hours, double minutes)
        {
            // Arrange
            var parking = new ParkingTransaction { ParkStart = DateTime.Now, ParkEnd = DateTime.Now.AddHours(hours).AddMinutes(minutes)};
            // Act
            var result = _parkingAccess.CalculateParkedTime(parking);
            // Assert
            Assert.IsType<TimeSpan>(result);    
            Assert.True(result.TotalMinutes >= (minutes+hours*60));
        }

        [Fact]
        public async Task CalculateRevenue_ReturnsCorrectRevenue()
        {     
            // Act
            var result = await _parkingAccess.CalculateRevenue();
            // Assert
            Assert.IsType<double>(result);
            Assert.Equal(100, result);

        }

        public void Dispose()
        {
            _mockContext.Dispose();
        }
    }
}
