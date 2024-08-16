using GarageCoreAPI.Models;
using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GarageCoreAPI.Tests.Controllers
{
    public class JobControllerTest
    {
        private readonly Mock<IRepair> _mockRepair;
        private readonly Mock<IGarage> _mockGarage;
        private readonly JobController _controller;

        public JobControllerTest()
        {
            _mockGarage = new Mock<IGarage>();
            _mockRepair = new Mock<IRepair>();
            _controller = new JobController(_mockRepair.Object, _mockGarage.Object);
        }

        [Fact]
        public async Task Info_ReturnsListOfJobs()
        {
            // Arrange
            List<Job> jobs = new List<Job>([Mock.Of<Job>()]);
            _mockRepair.Setup(m => m.ListJobs()).ReturnsAsync(jobs);

            // Act
            var result = await _controller.Info();

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
            Assert.IsType<List<Job>>(jsonResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenJobIsNull()
        {
            // Arrange
            Job? job = null;

            // Act
            var result = await _controller.Create(job);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.JobDataUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsJsonWithSuccessFalse_WhenJobDataIsInvalid()
        {
            // Arrange
            Job job = Mock.Of<Job>();
            _mockRepair.Setup(m => m.CheckValidityDirect(job)).Returns(false);

            // Act
            var result = await _controller.Create(job);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.ValidationFailedMessage, response.Message);
        }

        [Fact]
        public async Task Create_ReturnsJsonWithSuccessFalse_WhenJobAlreadyExists()
        {
            // Arrange
            Job job = Mock.Of<Job>();
            _mockRepair.Setup(m => m.CheckValidityDirect(job)).Returns(true);
            _mockRepair.Setup(m => m.DoesJobExist(job.Name)).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(job);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"A spot with <{job.Name}> already exist !", response.Message);
        }

        [Fact]
        public async Task Create_ReturnsJsonWithSuccessTrue_WhenUniqueJobIsCreated()
        {
            // Arrange
            Job job = Mock.Of<Job>();
            _mockRepair.Setup(m => m.CheckValidityDirect(job)).Returns(true);
            _mockRepair.Setup(m => m.DoesJobExist(job.Name)).ReturnsAsync(false);

            // Act
            var result = await _controller.Create(job);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"New job - {job.Name} is created !", response.Message);
        }

        [Fact]
        public async Task GetAssignedJobs_ReturnsBadRequest_WhenVehicleIDIsNull()
        {
            // Arrange
            string? vehicleID = null;   

            // Act
            var result = await _controller.GetAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleIDInRepairDtoUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task GetAssignedJobs_ReturnsEmptyList_WhenTransactionsDontExist()
        {
            // Arrange
            string? vehicleID = GarageDummyData.FindableVehicleID;
            var job = GarageDummyData.NewJob;
            var transaction = Mock.Of<RepairTransaction>();
            _mockRepair.Setup(m => m.GetRepairTransactionsByVID(vehicleID)).ReturnsAsync([transaction]);
            _mockRepair.Setup(m => m.GetJobByID(transaction.ID)).ReturnsAsync(job);

            // Act
            var result = await _controller.GetAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<List<RepairTransactionDto>>(jsonResult.Value);
            Assert.Empty(response);
        }

        [Fact]
        public async Task GetAssignedJobs_ReturnsListOfRepairDtos_WhenTransactionsExist()
        {
            // Arrange
            string? vehicleID = GarageDummyData.FindableVehicleID;
            var job = GarageDummyData.NewJob;
            var transaction = new RepairTransaction() { JobID = job.ID, VehicleID = vehicleID, Charge = job.Charge, IsCompleted = false };
            _mockRepair.Setup(m => m.GetRepairTransactionsByVID(vehicleID)).ReturnsAsync([transaction]);
            _mockRepair.Setup(m => m.GetJobByID(transaction.JobID)).ReturnsAsync(job);

            // Act
            var result = await _controller.GetAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<List<RepairTransactionDto>>(jsonResult.Value);
            Assert.Single(response);
            Assert.Equal(job.Name, response[0].JobName);
        }

        [Fact]
        public async Task AssignJob_ReturnsBadRequest_WhenRepairDtoIsIncompleteOrNull()
        {
            // Arrange
            RepairDto repairDto = Mock.Of<RepairDto>();

            // Act
            var result = await _controller.AssignJob(repairDto);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleIDInRepairDtoUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task AssignJob_ReturnsBadRequest_WhenRepairDtoHasEmptyJobList()
        {
            // Arrange
            RepairDto repairDto = new RepairDto()
            {
                vehicleID = "DU101",
                jobs = new List<Job>()
            };

            // Act
            var result = await _controller.AssignJob(repairDto);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.JobListUnavailableToAssignMessage, badResult.Value);
        }

        [Fact]
        public async Task AssignJob_ReturnsJsonWithSuccessTrue_WhenRepairDtoIsIncompleteOrNull()
        {
            // Arrange
            RepairDto repairDto = new RepairDto()
            {
                vehicleID = "DU101",
                jobs = new List<Job>([Mock.Of<Job>()])
            };

            // Act
            var result = await _controller.AssignJob(repairDto);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.JobsAssignedSuccessMessage, response.Message);
        }

        [Fact]
        public async Task CompleteAssignedJobs_ReturnsBadRequest_WhenVehicleIDIsNullOrEmpty()
        {
            // Arrange
            string? vehicleID = null;

            // Act
            var result = await _controller.GetAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleIDInRepairDtoUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task CompleteAssignedJobs_ReturnsBadRequest_WhenVehicleIDIsEmpty()
        {
            // Arrange
            string vehicleID = string.Empty;

            // Act
            var result = await _controller.GetAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleIDInRepairDtoUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task CompleteAssignedJobs_ReturnsJsonWithSuccessTrue_WhenPendingTransactionListIsEmpty()
        {
            // Arrange
            string? vehicleID = "DU101";
            _mockRepair.Setup(m => m.GetPendingTransactionsByJID(vehicleID)).ReturnsAsync([]);

            // Act
            var result = await _controller.CompleteAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.NoPendingJobsMessage, response.Message);

        }

        [Fact]
        public async Task CompleteAssignedJobs_ReturnsJsonWithSuccessTrue_WhenPendingTransactionsExist()
        {
            // Arrange
            string? vehicleID = "DU101";
            _mockRepair.Setup(m => m.GetPendingTransactionsByJID(vehicleID)).ReturnsAsync([Mock.Of<RepairTransaction>()]);

            // Act
            var result = await _controller.CompleteAssignedJobs(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.JobsCompletedSuccessMessage, response.Message);

        }

        [Fact]
        public async Task RemoveJob_ReturnsBadRequest_WhenJobIDIsNullOrEmpty()
        {
            // Arrange
            string? jobID = null;

            // Act
            var result = await _controller.RemoveJob(jobID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Job with ID: {jobID} is not found on the server !", badResult.Value);
        }

        [Fact]
        public async Task RemoveJob_ReturnsJsonWithSuccessTrue_WhenPendingTransactionListIsEmpty()
        {
            // Arrange
            string? vehicleID = "DU101";
            _mockRepair.Setup(m => m.GetPendingTransactionsByJID(vehicleID)).ReturnsAsync([]);

            // Act
            var result = await _controller.RemoveJob(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.JobsDeletedSuccessMessage, response.Message);

        }

        [Fact]
        public async Task RemoveJob_ReturnsJsonWithSuccessTrue_WhenPendingTransactionsExist()
        {
            // Arrange
            string? vehicleID = "DU101";
            _mockRepair.Setup(m => m.GetPendingTransactionsByJID(vehicleID)).ReturnsAsync([Mock.Of<RepairTransaction>()]);

            // Act
            var result = await _controller.RemoveJob(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.JobsRestrictDeletionMessage, response.Message);

        }

        [Fact]
        public async Task UploadJobImage_ReturnsBadRequest_WhenJobIDIsNullOrEmpty()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _controller.UploadJobImage(file);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.FileUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task UploadJobImage_ReturnsJsonWithSuccessTrue_WhenPendingTransactionListIsEmpty()
        {
            // Arrange
            IFormFile? file = Mock.Of<IFormFile>();

            // Act
            var result = await _controller.UploadJobImage(file);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.ImageUploadedSuccessMessage, response.Message);
        }

        [Fact]
        public async Task GetUploadedJobImage_ReturnsJobNotFound_WhenJobIDIsInvalid()
        {
            // Arrange
            string jobID = "job1";
            _mockRepair.Setup(m => m.GetJobByID(jobID)).ReturnsAsync(null as Job);

            // Act
            var result = await _controller.GetUploadedJobImage(jobID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(DefaultValues.JobAssociatedToIDNotFoundMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task GetUploadedJobImage_ReturnsImageNotFound_WhenJobImageIsUnavailable()
        {
            // Arrange
            string jobID = "job1";
            _mockRepair.Setup(m => m.GetJobByID(jobID)).ReturnsAsync(Mock.Of<Job>());

            // Act
            var result = await _controller.GetUploadedJobImage(jobID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(DefaultValues.ImageNotFoundMessage, notFoundResult.Value);
        }
    }
}