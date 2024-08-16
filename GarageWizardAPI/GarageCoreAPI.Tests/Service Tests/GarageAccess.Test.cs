using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Common;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Xml.Linq;

namespace GarageCoreAPI.Tests.Services
{
    public class GarageAccessTest: IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MockGarageDBContext _mockContext;
        private readonly GarageAccess _garageAccess;

        public GarageAccessTest()
        {
            _mockContext = new MockGarageDBContext(null);
            _mockCache = new Mock<IMemoryCache>();
            _mockContext.SeedDatabaseContext();
            _garageAccess = new GarageAccess(_mockContext, _mockCache.Object);
        }

        [Fact]
        public async Task GetVehicles_ReturnsListOfVehicles_WhenCacheIsMissed()
        {
            // Arrange
            object vehicles = _mockContext.Vehicles.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out vehicles)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _garageAccess.GetVehicles();

            // Assert
            Assert.IsType<List<Vehicle>>(result);
            Assert.Equal(GarageDummyData.Vehicles.Count(), result.Count());
        }

        [Fact]
        public async Task GetVehicles_ReturnsListOfVehicles_WhenCacheIsHit()
        {
            // Arrange
            object vehicles = _mockContext.Vehicles.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out vehicles)).Returns(true);

            // Act
            var result = await _garageAccess.GetVehicles();

            // Assert
            Assert.IsType<List<Vehicle>>(result);
            Assert.Equal(GarageDummyData.Vehicles.Count(), result.Count());
        }

        [Fact]
        public async Task GetVehiclesNotOnRepair_ReturnsListOfVehicles()
        {
            // Act
            var result = await _garageAccess.GetVehiclesNotOnRepair();
            // Assert
            Assert.IsType<List<Vehicle>>(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SetVehicleOnRepairFlag_ReturnSuccessFalse_WhenVehicleNotFound()
        {
            // Act
            await _garageAccess.SetVehicleOnRepairFlag(GarageDummyData.NotFindableVehicleID, true);
            // Assert
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.NotFindableVehicleID);
            Assert.Null(vehicle);
        }

        [Fact]
        public async Task SetVehicleOnRepairFlag_ReturnSuccessTrue_WhenVehicleFound()
        {
            // Act
            await _garageAccess.SetVehicleOnRepairFlag(GarageDummyData.FindableVehicleID, true);
            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.FindableVehicleID);
            Assert.IsType<Vehicle>(vehicle);
            Assert.NotNull(vehicle);
            Assert.True(vehicle.IsOnRepair);
        }

        [Fact]
        public async Task GetVehicleByID_ReturnVehicle_WhenFound()
        {
            // Act
            var result = await _garageAccess.GetVehicleByID(GarageDummyData.FindableVehicleID);
            // Assert
            Assert.IsType<Vehicle>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetVehicleByID_ReturnNull_WhenVehicleNotFound()
        {
            // Act
            var result = await _garageAccess.GetVehicleByID(GarageDummyData.NotFindableVehicleID);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ParkVehicle_SuccessfulPark()
        {
            // Act
            await _garageAccess.ParkVehicle(GarageDummyData.NewVehicle);
            // Assert
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.NewVehicle.RegID);
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            Assert.IsType<Vehicle>(vehicle);
            Assert.NotNull(vehicle);
        }

        [Fact]
        public async Task RemoveVehicle_UnableToRemove_WhenVehicleNotFound()
        {
            // Act
            await _garageAccess.RemoveVehicle(GarageDummyData.NotFindableVehicleID);
            // Assert
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.NotFindableVehicleID);
            Assert.Null(vehicle);
        }

        [Fact]
        public async Task RemoveVehicle_SuccessfulRemoval_WhenVehicleFound()
        {
            // Act
            await _garageAccess.RemoveVehicle(GarageDummyData.FindableVehicleID);
            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.FindableVehicleID);
            Assert.Null(vehicle);
        }

        [Fact]
        public async Task DoesVehicleExist_ReturnTrue_WhenVehicleFound()
        {
            // Act
            var result = await _garageAccess.DoesVehicleExist(GarageDummyData.FindableVehicleID);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DoesVehicleExist_ReturnFalse_WhenVehicleNotFound()
        {
            // Act
            var result = await _garageAccess.DoesVehicleExist(GarageDummyData.NotFindableVehicleID);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateVehicle_UnableToUpdate_WhenVehicleNotFound()
        {
            // Arrange
            var updatedVehicle = new Vehicle() { Name = "Kummy", RegID = GarageDummyData.NotFindableVehicleID, IsOnRepair = false, Vendor = "Kumbo", Wheels = 4 };
            // Act
            await _garageAccess.UpdateVehicle(updatedVehicle);
            // Assert
            var vehicle = await _mockContext.Vehicles.FindAsync(GarageDummyData.NotFindableVehicleID);
            Assert.Null(vehicle);
        }

        [Fact]
        public async Task UpdateVehicle_SuccessfulUpdate_WhenVehicleFound()
        {
            // Arrange
            var updatedVehicle = new Vehicle() { Name = "Kummy", RegID = "DU102", IsOnRepair = false, Vendor = "Kumbo", Wheels = 4 };
            // Act
            await _garageAccess.UpdateVehicle(updatedVehicle);
            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var vehicle = await _mockContext.Vehicles.FindAsync("DU102");
            Assert.NotNull(vehicle);
            Assert.IsType<Vehicle>(vehicle);
            Assert.Equal(updatedVehicle, vehicle);
        }

        [Fact]
        public void CheckValidityDirect_ReturnTrue_NoExceptionCatched()
        {
            // Act
            var success = _garageAccess.CheckValidityDirect(GarageDummyData.NewVehicle);
            // Assert
            Assert.True(success);
        }

        [Fact]
        public void ToString_ReturnsString_VehicleModel()
        {
            // Arrange
            var updatedVehicle = GarageDummyData.NewVehicle;
            // Act
            var result = updatedVehicle.ToString();
            // Assert
            Assert.IsType<string>(result);
            Assert.Equal($"{updatedVehicle.RegID} | {updatedVehicle.Vendor} - {updatedVehicle.Name} ({updatedVehicle.Wheels}) ", result);
        }

        [Theory]
        [InlineData("Dummy", true)]
        [InlineData("Dummy1", true)]
        [InlineData("1Dummy", false)]
        [InlineData("dummy1", false)]
        [InlineData("DUMMY1", true)]
        public void CheckValidityDirect_TestVehicleName(string name, bool expected)
        {
            // Arrange
            var badVehicle = new Vehicle { Name = name, RegID = "DU101", Vendor = "Dumbo", Wheels = 4 };
            // Act
            var success = _garageAccess.CheckValidityDirect(badVehicle);
            // Assert
            Assert.Equal(expected, success);
        }

        [Theory]
        [InlineData("DU101", true)]
        [InlineData("D101", false)]
        [InlineData("10101", false)]
        [InlineData("dX101", false)]
        [InlineData("du101", false)]
        [InlineData("DU1012", false)]
        public void CheckValidityDirect_TestVehicleID(string id, bool expected)
        {
            // Arrange
            var badVehicle = new Vehicle { Name = "Dummy", RegID = id, Vendor = "Dumbo", Wheels = 4 };
            // Act
            var success = _garageAccess.CheckValidityDirect(badVehicle);
            // Assert
            Assert.Equal(expected, success);
        }

        [Theory]
        [InlineData("Dumbo", true)]
        [InlineData("Dumbo1", true)]
        [InlineData("1Dumbo", false)]
        [InlineData("dummbo1", false)]
        [InlineData("DUMBO1", true)]
        public void CheckValidityDirect_TestVehicleVendor(string vendor, bool expected)
        {
            // Arrange
            var badVehicle = new Vehicle { Name = "Dummy", RegID = "DU101", Vendor = vendor, Wheels = 4 };
            // Act
            var success = _garageAccess.CheckValidityDirect(badVehicle);
            // Assert
            Assert.Equal(expected, success);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void CheckValidityDirect_TestVehicleWheels(int wheels, bool expected)
        {
            // Arrange
            var badVehicle = new Vehicle { Name = "Dummy", RegID = "DU101", Vendor = "Dumbo", Wheels = wheels };
            // Act
            var success = _garageAccess.CheckValidityDirect(badVehicle);
            // Assert
            Assert.Equal(expected, success);
        }

        [Theory]
        [InlineData("Dummy", "DU101", "1Dumbo", false)]
        [InlineData("Dummy", "dX101", "Dumbo", false)]
        [InlineData("1Dummy", "DU101", "Dumbo", false)]
        [InlineData("Dummy", "DU101", "Dumbo1", true)]
        public void CheckValidityDirect_TestStringsTogether(string name, string id, string vendor, bool expected)
        {
            // Arrange
            var badVehicle = new Vehicle { Name = name, RegID = id, Vendor = vendor, Wheels = 4 };
            // Act
            var success = _garageAccess.CheckValidityDirect(badVehicle);
            // Assert
            Assert.Equal(expected, success);
        }

        public void Dispose()
        {
            _mockCache.Object.Dispose();    
            _mockContext.Dispose();
        }
    }
}
