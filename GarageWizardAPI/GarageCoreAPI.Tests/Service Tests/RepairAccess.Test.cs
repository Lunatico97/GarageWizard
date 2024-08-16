using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Common;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace GarageCoreAPI.Tests.Services
{
    public class RepairAccessTest: IDisposable
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly MockGarageDBContext _mockContext;
        private readonly RepairAccess _repairAccess;

        public RepairAccessTest()
        {
            _mockContext = new MockGarageDBContext(null);
            _mockCache = new Mock<IMemoryCache>();
            _mockContext.SeedDatabaseContext();
            _repairAccess = new RepairAccess(_mockContext, _mockCache.Object);
        }

        [Fact]
        public async Task CreateJob_ResetsCache_WhenSuccessfulCreation()
        {
            // Arrange
            Job newJob = GarageDummyData.NewJob;

            // Act
            await _repairAccess.CreateJob(newJob.Name, newJob.Description, newJob.Hours, newJob.Charge, newJob.JobImagePath);

            // Assert
            var job = await _repairAccess.GetJobByName(newJob.Name);
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            Assert.NotNull(job);
            Assert.IsType<Job>(job);
            Assert.Equal(newJob.Description, job.Description);
            Assert.Equal(newJob.Hours, job.Hours);
            Assert.Equal(newJob.Charge, job.Charge);
            Assert.Equal(newJob.JobImagePath, job.JobImagePath);
        }

        [Fact]
        public async Task CreateRepairTransactions_AddsRepairTransactions()
        {
            // Arrange
            var vehicleID = GarageDummyData.FindableVehicleID;
            var jobs = new List<Job>
            {
                new Job { ID = "j3", Charge = 50 },
                new Job { ID = "j4", Charge = 100 }
            };

            // Act
            await _repairAccess.CreateRepairTransactions(vehicleID, jobs);

            // Assert
            var repairs = await _mockContext.Repairs.ToListAsync();
            Assert.Equal(2, repairs.Count);
            Assert.All(repairs, r => Assert.Equal(vehicleID, r.VehicleID));
        }

        [Fact]
        public async Task GetAllRepairTransactions_ReturnsListOfRepairs()
        {
            // Act
            var result = await _repairAccess.GetAllRepairTransactions();
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<RepairTransaction>>(result);
        }

        [Fact]
        public async Task GetPendingTransactionsByJID_ReturnsEmpty_WhenJobNotFound()
        {
            // Act
            var result = await _repairAccess.GetPendingTransactionsByJID(GarageDummyData.NotFindableJobID);
            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPendingTransactionsByJID_ReturnsRepair_WhenJobFound()
        {
            // Act
            var result = await _repairAccess.GetPendingTransactionsByJID(GarageDummyData.FindableJobID);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<RepairTransaction>>(result);
        }

        [Fact]
        public async Task GetRepairTransactionsByVID_ReturnsEmpty_WhenVehicleNotFound()
        {
            // Act
            var result = await _repairAccess.GetRepairTransactionsByVID(GarageDummyData.NotFindableJobID);
            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetRepairTransactionsByVID_ReturnsRepair_WhenVehicleFound()
        {
            // Act
            var result = await _repairAccess.GetRepairTransactionsByVID(GarageDummyData.FindableJobID);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<RepairTransaction>>(result);
        }

        [Fact]
        public async Task GetJobByID_ReturnsNull_WhenJobNotFound()
        {
            // Act
            var result = await _repairAccess.GetJobByID(GarageDummyData.NotFindableJobID);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetJobByID_ReturnsJob_WhenJobFound()
        {
            // Act
            var result = await _repairAccess.GetJobByID(GarageDummyData.FindableJobID);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<Job>(result);
        }

        [Fact]
        public async Task ArchiveRepairTransactions_UpdatesVehicleIDToNull()
        {
            // Arrange
            var vehicleID = GarageDummyData.FindableVehicleID;
            await _repairAccess.CreateRepairTransactions(vehicleID, new List<Job> { new Job { ID = "z1", Charge = 50 } });

            // Act
            await _repairAccess.ArchiveRepairTransactions(vehicleID);

            // Assert
            var transactions = await _mockContext.Repairs.ToListAsync();
            Assert.All(transactions, t => Assert.Null(t.VehicleID));
        }

        [Fact]
        public async Task CompleteRepairTransactions_MarksTransactionsAsCompleted()
        {
            // Arrange
            var vehicleID = GarageDummyData.FindableVehicleID;
            await _repairAccess.CreateRepairTransactions(vehicleID, new List<Job> { new Job { ID = "z1", Charge = 50 } });

            // Act
            await _repairAccess.CompleteRepairTransactions(vehicleID);

            // Assert
            var transactions = await _mockContext.Repairs.ToListAsync();
            Assert.All(transactions, t => Assert.True(t.IsCompleted));
        }

        [Fact]
        public async Task DeleteJob_UnableToRemove_WhenJobNotFound()
        {
            // Act
            await _repairAccess.DeleteJob(GarageDummyData.NotFindableJobID);

            // Assert
            var deletedJob = await _mockContext.Jobs.FindAsync(GarageDummyData.NotFindableJobID);
            Assert.Null(deletedJob);
        }

        [Fact]
        public async Task DeleteJob_RemovesJobAndResetsCache_WhenJobFound()
        {
            // Arrange
            var job = new Job { ID = "job123", Name = "Job to Delete", Charge = 50 };
            _mockContext.Jobs.Add(job);
            await _mockContext.SaveChangesAsync();

            // Act
            await _repairAccess.DeleteJob(job.ID);

            // Assert
            _mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Once);
            var deletedJob = await _mockContext.Jobs.FindAsync(GarageDummyData.NotFindableJobID);
            Assert.Null(deletedJob);   
        }

        [Fact]
        public async Task ListJobs_ReturnsListOfJobs_WhenCacheIsMissed()
        {
            // Arrange
            object jobs = await _mockContext.Jobs.ToListAsync();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out jobs)).Returns(false);
            _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>);

            // Act
            var result = await _repairAccess.ListJobs();

            // Assert
            Assert.IsType<List<Job>>(result);
            Assert.Equal(GarageDummyData.Jobs.Count(), result.Count());
        }

        [Fact]
        public async Task ListJobs_ReturnsListOfJobs_WhenCacheIsHit()
        {
            // Arrange
            object jobs = await _mockContext.Jobs.ToListAsync();
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<object>(), out jobs)).Returns(true);
            // Act
            var result = await _repairAccess.ListJobs();

            // Assert
            Assert.IsType<List<Job>>(result);
            Assert.Equal(GarageDummyData.Jobs.Count(), result.Count());
        }

        [Fact]
        public async Task UpdateJob_UpdatesJobAndRemovesCache()
        {
            // Arrange
            var job = new Job { ID = "job123", Name = "Job to Update", Charge = 50 };
            _mockContext.Jobs.Add(job);
            await _mockContext.SaveChangesAsync();

            // Act
            await _repairAccess.UpdateJob(job.ID, "Updated Name", "Updated Description", 100, "/new/path");

            // Assert
            var updatedJob = await _mockContext.Jobs.FindAsync(job.ID);
            Assert.NotNull(updatedJob);
            Assert.Equal("Updated Name", updatedJob.Name);
            Assert.Equal("Updated Description", updatedJob.Description);
            Assert.Equal(100, updatedJob.Charge);
            Assert.Equal("/new/path", updatedJob.JobImagePath);
            _mockCache.Verify(m => m.Remove(Constants.JobCacheKey), Times.Once);
        }

        [Fact]
        public async Task DoesJobExist_ReturnsFalse_WhenJobNotFound()
        {
            // Arrange
            string jobName = "Xob1";
            // Act
            var success = await _repairAccess.DoesJobExist(jobName);
            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task DoesJobExist_ReturnsTrue_WhenJobFound()
        {
            // Arrange
            string jobName = "Job1";
            // Act
            var success = await _repairAccess.DoesJobExist(jobName);
            // Assert
            Assert.True(success);
        }

        [Fact]
        public async Task CalculateRevenue_ReturnsCorrectRevenue()
        {
            // Arrange
            var job1 = new Job { ID = "job1", Charge = 100 };
            var job2 = new Job { ID = "job2", Charge = 200 };
            var transaction1 = new RepairTransaction { ID = "r1", JobID = "job1", Charge = 100, job = null, vehicle = null };
            var transaction2 = new RepairTransaction { ID = "r2", JobID = "job2", Charge = 200, job = null, vehicle = null };
            _mockContext.Jobs.AddRange(job1, job2);
            _mockContext.Repairs.AddRange(transaction1, transaction2);
            await _mockContext.SaveChangesAsync();

            // Act
            var revenue = await _repairAccess.CalculateRevenue();

            // Assert
            Assert.Equal(300, revenue);
        }

        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public void CheckValidityDirect_TestCharge(int charge, bool expected)
        {
            // Arrange
            var badJob = new Job() { Name = "JobX", Charge = charge, Hours = 5, Description = "I can do this all day !", JobImagePath = "path/to/job"};
            // Act
            var success = _repairAccess.CheckValidityDirect(badJob);
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
