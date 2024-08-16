using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace GarageCoreAPI.Tests.Services
{
    public class TollAccessTest : IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MockGarageDBContext _mockContext;
        private readonly TollAccess _tollAccess;

        public TollAccessTest()
        {
            _mockContext = new MockGarageDBContext(null);
            _mockCache = new Mock<IMemoryCache>();
            _mockContext.SeedDatabaseContext();
            _tollAccess = new TollAccess(_mockContext, _mockCache.Object);
        }

        [Fact]
        public async Task GetSpots_ReturnsListOfSpots_WhenCacheIsMissed()
        {
            // Arrange
            object spots = _mockContext.Spots.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out spots)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            // Act
            var result = await _tollAccess.GetSpots();

            // Assert
            Assert.IsType<List<Spot>>(result);
            Assert.Equal(GarageDummyData.Spots.Count(), result.Count());
        }

        [Fact]
        public async Task GetVehicles_ReturnsListOfSpots_WhenCacheIsHit()
        {
            // Arrange
            object spots = _mockContext.Spots.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out spots)).Returns(true);

            // Act
            var result = await _tollAccess.GetSpots();

            // Assert
            Assert.IsType<List<Spot>>(result);
            Assert.Equal(GarageDummyData.Spots.Count(), result.Count());
        }

        [Fact]
        public async Task GetVacantSpots_ReturnsListOfSpots()
        {
            object spots = _mockContext.Spots.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out spots)).Returns(true);
            // Act
            var result = await _tollAccess.GetVacantSpots();
            // Assert
            Assert.IsType<List<Spot>>(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetVacantUsableSpots_ReturnsListOfSpots()
        {
            // Arrange
            int capacity = 2;
            object spots = _mockContext.Spots.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out spots)).Returns(true);
            // Act
            var result = await _tollAccess.GetVacantUsableSpots(capacity);
            // Assert
            Assert.IsType<List<Spot>>(result);
            Assert.Single(result);
            Assert.Equal(8, result[0].Capacity);
        }

        [Fact]
        public async Task AreThereVacantSpots_ReturnsTrue_WhenAvailable()
        {
            // Arrange
            object spots = _mockContext.Spots.ToList();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out spots)).Returns(true);
            // Act
            var result = await _tollAccess.AreThereVacantSpots();
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsThisSpotVacant_ReturnsTrue_WhenAvailable()
        {
            // Act
            var result = await _tollAccess.IsThisSpotVacant(GarageDummyData.FindableSpotID);
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsThisSpotVacant_ReturnsFalse_WhenNotAvailable()
        {
            // Act
            var result = await _tollAccess.IsThisSpotVacant(GarageDummyData.NotFindableSpotID);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetSpotByID_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _tollAccess.GetSpotByID(GarageDummyData.NotFindableSpotID);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSpotByID_ReturnsNull_WhenSpotFound()
        {
            // Act
            var result = await _tollAccess.GetSpotByID(GarageDummyData.FindableSpotID);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<Spot>(result);
        }

        [Fact]
        public async Task GetSpotByVID_ReturnsNull_WhenNotFound()
        {
            // Act
            var result = await _tollAccess.GetSpotByVID(GarageDummyData.NotFindableVehicleID);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSpotByVID_ReturnsSpot_WhenSpotFound()
        {
            // Act
            var result = await _tollAccess.GetSpotByVID(GarageDummyData.FindableVehicleID);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<Spot>(result);
        }

        [Fact]
        public async Task UpdateSpotOccupancy_UnableToUpdate_WhenNotFound()
        {
            // Act
            await _tollAccess.UpdateSpotOccupancy(GarageDummyData.NotFindableSpotID, false);
            // Assert
            var spot = await _mockContext.Spots.FindAsync(GarageDummyData.NotFindableSpotID);
            Assert.Null(spot);
        }

        [Fact]
        public async Task UpdateSpotOccupancy_SuccesfulUpdate_WhenSpotFound()
        {
            // Act
            await _tollAccess.UpdateSpotOccupancy(GarageDummyData.FindableSpotID, false);
            // Assert
            var spot = await _mockContext.Spots.FindAsync(GarageDummyData.FindableSpotID);
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            Assert.NotNull(spot);
            Assert.IsType<Spot>(spot);
            Assert.False(spot.Occupied);
        }

        [Fact]
        public async Task AddSpot_ResetsCache_WhenSucccessfulInsertion()
        {
            // Arrange
            string spotID = GarageDummyData.NewSpot.ID;
            int capacity = GarageDummyData.NewSpot.Capacity;
            // Act
            await _tollAccess.AddSpot(spotID, capacity);
            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var spot = await _mockContext.Spots.FindAsync(spotID);
            Assert.NotNull(spot);
            Assert.IsType<Spot>(spot);
            Assert.False(spot.Occupied);
            Assert.Equal(capacity, spot.Capacity);
            Assert.Equal(spotID, spot.ID);
        }

        [Fact]
        public async Task Removal_SuccessfulRemoval_WhenSpotNotFound()
        {
            // Arrange
            string spotID = GarageDummyData.NotFindableSpotID;
            // Act
            await _tollAccess.RemoveSpot(spotID);
            // Assert
            var spot = await _mockContext.Spots.FindAsync(spotID);
            Assert.Null(spot);
        }

        [Fact]
        public async Task Removal_SuccessfulRemoval_WhenSpotFound()
        {
            // Arrange
            string spotID = GarageDummyData.FindableSpotID;
            // Act
            await _tollAccess.RemoveSpot(spotID);
            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var spot = await _mockContext.Spots.FindAsync(spotID);
            Assert.Null(spot);
        }

        [Fact]
        public async Task CheckIfSpotExists_ReturnsTrue_WhenSpotFound()
        {
            // Act
            var success = await _tollAccess.CheckIfSpotExists(GarageDummyData.FindableSpotID);
            // Arrange
            Assert.True(success);
        }

        [Fact]
        public async Task CheckIfSpotExists_ReturnsFalse_WhenSpotFound()
        {
            // Act
            var success = await _tollAccess.CheckIfSpotExists(GarageDummyData.NotFindableSpotID);
            // Arrange
            Assert.False(success);
        }

        [Fact]
        public void ToString_ReturnsString_SpotModel()
        {
            // Arrange
            var spot = GarageDummyData.NewSpot;
            // Act
            var result = spot.ToString();
            // Assert
            Assert.IsType<string>(result);
            Assert.Equal($"{spot.ID} [ Capacity: {spot.Capacity} wheeler ]", result);

        }

        [Theory]
        [InlineData("D101", true)]
        [InlineData("DMAX", true)]
        [InlineData("d101", false)]
        [InlineData("DM01", true)]
        [InlineData("1010", false)]
        [InlineData("D1012", false)]
        public void CheckValidityDirect_TestSpotID(string id, bool expected)
        {
            // Arrange
            var badSpot = new Spot { ID = id, Capacity = 4, Occupied = false };
            // Act
            var success = _tollAccess.CheckValidityDirect(badSpot);
            // Assert
            Assert.Equal(expected, success);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData(-1, false)]
        public void CheckValidityDirect_TestSpotCapacity(int capacity, bool expected)
        {
            // Arrange
            var badSpot = new Spot { ID = "D101", Capacity = capacity, Occupied = false };
            // Act
            var success = _tollAccess.CheckValidityDirect(badSpot);
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
