using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GarageCoreAPI.Tests.Controllers
{
    public class HomeControllerTest
    {
        private readonly HomeController _controller;

        public HomeControllerTest() { 
            _controller = new HomeController();
        }

        [Fact]
        public void Index_ReturnSuccessMessage()
        {
            // Act
            var result = _controller.Index();
            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal(DefaultValues.WelcomeMessage, response.Message);
            Assert.Equal((int) HttpStatusCode.OK, response.StatusCode); 
        }
    }
}
