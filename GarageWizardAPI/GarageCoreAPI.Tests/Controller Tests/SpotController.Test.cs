using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GarageCoreAPI.Tests.Controllers
{
    public class SpotControllerTest
    {
        private readonly Mock<IToll> _mockToll;
        private readonly Mock<IParking> _mockParking;
        private readonly SpotController _mockSpotController;

        public SpotControllerTest()
        {
            _mockToll = new Mock<IToll>();
            _mockParking = new Mock<IParking>();
            _mockSpotController = new SpotController(_mockToll.Object, _mockParking.Object);
        }

        [Fact]
        public async Task Info_ReturnsListOfSpots()
        {
            // Arrange
            List<Spot> spots = new List<Spot>([Mock.Of<Spot>()]);
            _mockToll.Setup(m => m.GetSpots()).ReturnsAsync(spots);

            // Act
            var result = await _mockSpotController.Info();
            
            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<List<Spot>>(jsonResult.Value);
        }

        [Fact]
        public async Task Add_ReturnsJsonWithSuccessFalse_WhenSpotIsNullorInvalid()
        {
            // Arrange
            Spot? spot = null; 
            _mockToll.Setup(m => m.CheckValidityDirect(spot)).Returns(false);

            // Act
            var result = await _mockSpotController.Add(spot);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.ValidationFailedMessage, response.Message);
        }

        [Fact]
        public async Task Add_ReturnsJsonWithSuccessFalse_WhenValidSpotAlreadyExists()
        {
            // Arrange
            Spot spot = Mock.Of<Spot>();
            _mockToll.Setup(m => m.CheckValidityDirect(spot)).Returns(true);
            _mockToll.Setup(m => m.CheckIfSpotExists(spot.ID)).ReturnsAsync(true);  

            // Act
            var result = await _mockSpotController.Add(spot);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"A spot with <{spot.ID}> already exist !", response.Message);
        }

        [Fact]
        public async Task Add_ReturnsJsonWithSuccessTrue_WhenValidSpotIsUnique()
        {
            // Arrange
            Spot spot = Mock.Of<Spot>();
            _mockToll.Setup(m => m.CheckValidityDirect(spot)).Returns(true);
            _mockToll.Setup(m => m.CheckIfSpotExists(spot.ID)).ReturnsAsync(false);

            // Act
            var result = await _mockSpotController.Add(spot);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Spot with <{spot.ID} is added successfully !", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessFalse_WhenSpotIsNotVacant()
        {
            // Arrange
            string spotID = "D101";
            _mockToll.Setup(m => m.IsThisSpotVacant(spotID)).ReturnsAsync(false);

            // Act
            var result = await _mockSpotController.Remove(spotID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"The spot with <{spotID}> is not vacant so, it can't be deleted !", response.Message);
        }

        [Fact]
        public async Task Remove_ReturnsJsonWithSuccessTrue_WhenSpotIsVacant()
        {
            // Arrange
            string spotID = "D101";
            _mockToll.Setup(m => m.IsThisSpotVacant(spotID)).ReturnsAsync(true);

            // Act
            var result = await _mockSpotController.Remove(spotID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Spot with <{spotID}> is deleted !!", response.Message);
        }

        [Fact]
        public async Task GetSpotsToReserve_ReturnsListOfSpots()
        {
            // Arrange
            int wheels = 2;
            string vehicleID = "DU101";
            List<Spot> spots = new List<Spot>([Mock.Of<Spot>()]);
            _mockToll.Setup(m => m.GetVacantUsableSpots(wheels)).ReturnsAsync(spots);

            // Act
            var result = await _mockSpotController.CheckSpotsToReserve(vehicleID, wheels);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<List<Spot>>(jsonResult.Value);
        }

        [Fact]
        public async Task Reserve_ReturnsJsonWithSuccessFalse_WhenAllSpotsAreOccupied()
        {
            // Arrange
            Reservation reservation = Mock.Of<Reservation>();
            _mockToll.Setup(m => m.AreThereVacantSpots()).ReturnsAsync(false);

            // Act
            var result = await _mockSpotController.Reserve(reservation);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.ParkingSpaceFullMessage, response.Message);
        }

        [Fact]
        public async Task Reserve_ReturnsJsonWithSuccessFalse_WhenSpotsAreAvailableButReservationIsNullOrSpotIDEmpty()
        {
            // Arrange
            Reservation reservation = Mock.Of<Reservation>();
            _mockToll.Setup(m => m.AreThereVacantSpots()).ReturnsAsync(true);

            // Act
            var result = await _mockSpotController.Reserve(reservation);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal(DefaultValues.SpotIDNotProvidedMessage, response.Message);
        }

        [Fact]
        public async Task Reserve_ReturnsJsonWithSuccessFalse_WhenSpotToReserveIsOccupied()
        {
            // Arrange
            Reservation reservation = new Reservation()
            {
                spotID = "D101", vehicleID = "DU101", service = 1
            };
            _mockToll.Setup(m => m.AreThereVacantSpots()).ReturnsAsync(true);
            _mockToll.Setup(m => m.IsThisSpotVacant(reservation.spotID)).ReturnsAsync(false);

            // Act
            var result = await _mockSpotController.Reserve(reservation);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);
            Assert.Equal($"The spot with <{reservation.spotID}> is already occupied !", response.Message);
        }

        [Fact]
        public async Task Reserve_ReturnsJsonWithSuccessTrue_WhenSpotIsFreeToReserve()
        {
            // Arrange
            Reservation reservation = new Reservation()
            {
                spotID = "D101", vehicleID = "DU101", service = 1
            };
            _mockToll.Setup(m => m.AreThereVacantSpots()).ReturnsAsync(true);
            _mockToll.Setup(m => m.IsThisSpotVacant(reservation.spotID)).ReturnsAsync(true);

            // Act
            var result = await _mockSpotController.Reserve(reservation);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Reservation successful for {reservation.vehicleID} at {reservation.spotID} !!", response.Message);
        }
    }
}
