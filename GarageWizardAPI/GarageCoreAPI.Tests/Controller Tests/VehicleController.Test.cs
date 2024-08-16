using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GarageCoreAPI.Tests.Controllers
{
    public class VehicleControllerTest
    {
        private readonly Mock<IGarage> _mockGarage;
        private readonly Mock<IToll> _mockToll;
        private readonly Mock<IParking> _mockParking;
        private readonly Mock<IRepair> _mockRepair;
        private readonly VehicleController _mockVehicleController;

        public VehicleControllerTest()
        {
            _mockGarage = new Mock<IGarage>();
            _mockToll = new Mock<IToll>();
            _mockParking = new Mock<IParking>();
            _mockRepair = new Mock<IRepair>();
            _mockVehicleController = new VehicleController(_mockGarage.Object, _mockToll.Object, _mockParking.Object, _mockRepair.Object);
        }

        [Fact]
        public async Task Info_ReturnsListOfVehicles()
        {
            // Arrange
            List<Vehicle> vehicles = new List<Vehicle>([Mock.Of<Vehicle>()]);
            _mockGarage.Setup(m => m.GetVehicles()).ReturnsAsync(vehicles);

            // Act
            var result = await _mockVehicleController.Info();

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<List<Vehicle>>(jsonResult.Value);
        }

        [Fact]
        public async Task InfoNotOnRepair_ReturnsListOfVehicles()
        {
            // Arrange
            List<Vehicle> vehicles = new List<Vehicle>([Mock.Of<Vehicle>()]);
            _mockGarage.Setup(m => m.GetVehiclesNotOnRepair()).ReturnsAsync(vehicles);

            // Act
            var result = await _mockVehicleController.InfoNotOnRepair();

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<List<Vehicle>>(jsonResult.Value);
        }

        [Fact]
        public async Task Park_ReturnsBadRequest_WhenVehicleDataIsNull()
        {
            // Arrange
            Vehicle? vehicle = null;

            // Act
            var result = await _mockVehicleController.Park(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleDataUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task Park_ReturnsJsonWithSuccessFalse_WhenVehicleDataIsInvalid()
        {
            // Arrange
            Vehicle vehicle = Mock.Of<Vehicle>();
            _mockGarage.Setup(m => m.CheckValidityDirect(vehicle)).Returns(false);

            // Act
            var result = await _mockVehicleController.Park(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.ValidationFailedMessage, response.Message);
        }

        [Fact]
        public async Task Park_ReturnsJsonWithSuccessFalse_WhenDuplicateVehicleWithSameIDisParked()
        {
            // Arrange
            Vehicle vehicle = Mock.Of<Vehicle>();
            _mockGarage.Setup(m => m.CheckValidityDirect(vehicle)).Returns(true);
            _mockGarage.Setup(m => m.DoesVehicleExist(vehicle.RegID)).ReturnsAsync(true);

            // Act
            var result = await _mockVehicleController.Park(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.VehicleDuplicateErrorMessage, response.Message);
        }

        [Fact]
        public async Task Park_ReturnsJsonWithSuccessTrue_WhenVehicleWithUniqueIDisParked()
        {
            // Arrange
            Vehicle vehicle = Mock.Of<Vehicle>();
            _mockGarage.Setup(m => m.CheckValidityDirect(vehicle)).Returns(true);
            _mockGarage.Setup(m => m.DoesVehicleExist(vehicle.RegID)).ReturnsAsync(false);

            // Act
            var result = await _mockVehicleController.Park(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Vehicle with ID: {vehicle.RegID} is parked successfully !", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessFalse_WhenVehicleDoesntHaveSpot()
        {
            // Arrange
            string vehicleID = "DU101";
            _mockToll.Setup(m => m.GetSpotByVID(vehicleID)).ReturnsAsync(null as Spot);

            // Act
            var result = await _mockVehicleController.Remove(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Vehicle with ID: {vehicleID} is successfully removed! ", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessFalse_WhenVehicleDoesntExist()
        {
            // Arrange
            string vehicleID = "DU101";
            _mockToll.Setup(m => m.GetSpotByVID(vehicleID)).ReturnsAsync(Mock.Of<Spot>());
            _mockGarage.Setup(m => m.GetVehicleByID(vehicleID)).ReturnsAsync(null as Vehicle);

            // Act
            var result = await _mockVehicleController.Remove(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"Vehicle with ID: {vehicleID} doesn't exist ! ", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessFalse_WhenVehicleIsOnRepair()
        {
            // Arrange
            Vehicle vehicle = new Vehicle() { 
                Name = "Dummy", IsOnRepair = true, RegID = "DU101", Vendor = "Dumbo", Wheels = 4
            };
            _mockToll.Setup(m => m.GetSpotByVID(vehicle.RegID)).ReturnsAsync(Mock.Of<Spot>());
            _mockGarage.Setup(m => m.GetVehicleByID(vehicle.RegID)).ReturnsAsync(vehicle);

            // Act
            var result = await _mockVehicleController.Remove(vehicle.RegID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"Vehicle with ID: {vehicle.RegID} can't be removed due to pending repairs! ", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessTrue_WhenVehicleCanBeSuccessfullyRemoved()
        {
            // Arrange
            Vehicle vehicle = new Vehicle()
            {
                Name = "Dummy", IsOnRepair = false, RegID = "DU101", Vendor = "Dumbo", Wheels = 4
            };
            _mockToll.Setup(m => m.GetSpotByVID(vehicle.RegID)).ReturnsAsync(Mock.Of<Spot>());
            _mockGarage.Setup(m => m.GetVehicleByID(vehicle.RegID)).ReturnsAsync(vehicle);

            // Act
            var result = await _mockVehicleController.Remove(vehicle.RegID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Vehicle with ID: {vehicle.RegID} is successfully removed! ", response.Message);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_WhenVehicleDataIsNull()
        {
            // Arrange
            Vehicle? vehicle = null;

            // Act
            var result = await _mockVehicleController.Edit(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.VehicleDataUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task Edit_ReturnsJsonWithSuccessTrue_WhenVehicleDataIsInvalid()
        {
            // Arrange
            Vehicle vehicle = Mock.Of<Vehicle>();
            _mockGarage.Setup(m => m.CheckValidityDirect(vehicle)).Returns(false);

            // Act
            var result = await _mockVehicleController.Edit(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.ValidationFailedMessage, response.Message);
        }

        [Fact]
        public async Task Edit_ReturnsJsonWithSuccessTrue_WhenVehicleDataIsValid()
        {
            // Arrange
            Vehicle vehicle = Mock.Of<Vehicle>();
            _mockGarage.Setup(m => m.CheckValidityDirect(vehicle)).Returns(true);

            // Act
            var result = await _mockVehicleController.Edit(vehicle);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Vehicle with ID <{vehicle.RegID} is updated successfully !", response.Message);
        }

        [Fact]
        public async Task Specifics_ReturnsBadRequest_WhenVehicleWithIDDoesntExist()
        {
            // Arrange
            string vehicleID = "DU101";
            Spot spot = Mock.Of<Spot>();
            _mockGarage.Setup(m => m.GetVehicleByID(vehicleID)).ReturnsAsync(null as Vehicle);
            _mockToll.Setup(m => m.GetSpotByVID(vehicleID)).ReturnsAsync(spot);
            _mockParking.Setup(m => m.GetParkingByID(vehicleID, spot.ID)).ReturnsAsync(null as ParkingTransaction);

            // Act
            var result = await _mockVehicleController.GetSpecifics(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal($"Vehicle with ID: {vehicleID} doesn't exist !", badResult.Value);
        }

        [Fact]
        public async Task Specifics_ReturnsJsonWithVehicleDto_WhenVehicleExistsAndIsParked()
        {
            // Arrange
            string vehicleID = "DU101";
            Spot spot = Mock.Of<Spot>();
            _mockGarage.Setup(m => m.GetVehicleByID(vehicleID)).ReturnsAsync(Mock.Of<Vehicle>());
            _mockToll.Setup(m => m.GetSpotByVID(vehicleID)).ReturnsAsync(spot);
            _mockParking.Setup(m => m.GetParkingByID(spot.ID, vehicleID)).ReturnsAsync(Mock.Of<ParkingTransaction>());

            // Act
            var result = await _mockVehicleController.GetSpecifics(vehicleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<VehicleDto>(jsonResult.Value);
        }

        [Fact]
        public async Task GetStatistics_ReturnsStatisticsDto()
        {
            // Arrange 
            _mockGarage.Setup(m => m.GetVehicles()).ReturnsAsync(new List<Vehicle>([Mock.Of<Vehicle>()])) ;
            _mockGarage.Setup(m => m.GetVehiclesNotOnRepair()).ReturnsAsync(new List<Vehicle>([Mock.Of<Vehicle>()])) ;
            _mockToll.Setup(m => m.GetVacantSpots()).ReturnsAsync(new List<Spot>([Mock.Of<Spot>()])) ;

            // Act
            var result = await _mockVehicleController.GetStatistics();

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<StatisticsDto>(jsonResult.Value);
        }

        [Fact]
        public async Task GetParkingTimeSeriesData_ReturnsParkingByDayDto()
        {
            // Arrange 
            _mockRepair.Setup(m => m.GetAllRepairTransactions()).ReturnsAsync(new List<RepairTransaction>([Mock.Of<RepairTransaction>()]));
            _mockParking.Setup(m => m.GetParkingTransactions()).ReturnsAsync(new List<ParkingTransaction>([Mock.Of<ParkingTransaction>()]));

            // Act
            var result = await _mockVehicleController.GetParkingTimeSeriesData();

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<List<ParkingByDayDto>>(jsonResult.Value);
            Assert.IsType<ParkingByDayDto>(response[0]);
        }
    }
}
