using GarageCoreAPI.Models;
using GarageCoreAPI.Tests.Data;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using GarageCoreMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using System.Net;
using System.Xml.Linq;

namespace GarageCoreAPI.Tests.Controllers
{
    public class AdminControllerTest
    {
        private readonly AdminController _controller;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<Role>> _mockRoleManager;

        public AdminControllerTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockRoleManager = new Mock<RoleManager<Role>>(Mock.Of<IRoleStore<Role>>(), null, null, null, null);
            _controller = new AdminController(_mockRoleManager.Object, _mockUserManager.Object);
        }

        //[Fact]
        //public async Task ListRole_ReturnsListOfRoles()
        //{
        //    // Arrange
        //    var roles = GarageDummyData.Roles;
        //    _mockRoleManager.Setup(m => m.Roles).Returns(roles.AsQueryable<Role>());

        //    // Act
        //    var result = await _controller.ListRole();

        //    // Assert
        //    Assert.IsAssignableFrom<ActionResult>(result);
        //    var jsonResult = Assert.IsType<JsonResult>(result);
        //    Assert.IsType<List<Role>>(jsonResult.Value);
        //}

        [Fact]
        public async Task CreateRole_ReturnsBadRequest_WhenRoleDataIsNull()
        {
            // Arrange
            RoleView? role = null;

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.RoleDetailsUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task CreateRole_ReturnsJsonWithSuccessFalse_WhenRoleExists()
        {
            // Arrange
            RoleView role = Mock.Of<RoleView>();
            _mockRoleManager.Setup(m => m.RoleExistsAsync(role.Name)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(data.Success);
            Assert.Equal(DefaultValues.RoleExistsMessage, data.Message);
        }

        [Fact]
        public async Task CreateRole_ReturnsJsonWithSuccessFalse_WhenCreateAsyncFails()
        {
            // Arrange
            RoleView role = new RoleView() { Name = "DummyAdmin", Description = "I am a dummy !"};
            _mockRoleManager.Setup(m => m.RoleExistsAsync(role.Name)).ReturnsAsync(false);
            _mockRoleManager.Setup(m => m.CreateAsync(It.IsAny<Role>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = DefaultValues.RoleCreationFailedMessage}));

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(data.Success);
            Assert.Equal(DefaultValues.RoleCreationFailedMessage, data.Message);
        }

        [Fact]
        public async Task CreateRole_ReturnsJsonWithSuccessTrue_WhenCreateAsyncSucceeds()
        {
            // Arrange
            RoleView role = new RoleView() { Name = "DummyAdmin", Description = "I am a dummy !" };
            _mockRoleManager.Setup(m => m.RoleExistsAsync(role.Name)).ReturnsAsync(false);
            _mockRoleManager.Setup(m => m.CreateAsync(It.IsAny<Role>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.CreateRole(role);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(data.Success);
            Assert.Equal(DefaultValues.RoleCreationSuccessMessage, data.Message);
        }

        [Fact]
        public async Task GetRoleDetails_ReturnsNotFound_WhenRoleDoesntExist()
        {
            // Arrange
            string roleID = "role1";
            _mockRoleManager.Setup(m => m.FindByIdAsync(roleID)).ReturnsAsync(null as Role);

            // Act
            var result = await _controller.GetRoleDetails(roleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"The role with <{roleID}> doesn't exist !", notFoundResult.Value);
        }

        [Fact]
        public async Task GetRoleDetails_ReturnsRoleDetails_WhenRoleExists()
        {
            // Arrange
            string roleID = "role1";
            var users = new List<User>([
                new User() { Name = "Hero Zero", UserName = "Hero", Email = "hero@hero.com", RememberMe = false }
            ]); 
            _mockRoleManager.Setup(m => m.FindByIdAsync(roleID)).ReturnsAsync(Mock.Of<Role>());
            _mockUserManager.Setup(m => m.Users).Returns(users.AsQueryable<User>);
            _mockUserManager.Setup(m => m.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.GetRoleDetails(roleID);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<RoleDetails>(jsonResult.Value);
        }

        [Fact]
        public async Task EditRole_ReturnsBadRequest_WhenRoleDetailsIsNullToUpdate()
        {
            // Arrange
            RoleDetails? roleDetails = null;

            // Act
            var result = await _controller.EditRole(roleDetails);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.RoleDetailsUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task EditRole_ReturnsNotFound_WhenRoleDoesntExist()
        {
            // Arrange
            RoleDetails? roleDetails = Mock.Of<RoleDetails>();
            _mockRoleManager.Setup(m => m.FindByIdAsync(roleDetails.ID)).ReturnsAsync(null as Role);

            // Act
            var result = await _controller.EditRole(roleDetails);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal((int) HttpStatusCode.NotFound, notFoundResult.StatusCode);
            Assert.Equal($"Role with ID <{roleDetails.ID}> cannot be found! ", notFoundResult.Value); 
        }

        [Fact]
        public async Task EditRole_ReturnsJsonWithSuccessFalse_WhenUpdateAsyncFails()
        {
            // Arrange
            RoleDetails roleDetails = Mock.Of<RoleDetails>();
            Role role = Mock.Of<Role>();
            _mockRoleManager.Setup(m => m.FindByIdAsync(roleDetails.ID)).ReturnsAsync(role);
            _mockRoleManager.Setup(m => m.UpdateAsync(role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = $"{role.Name} cannot be updated properly !"} 
            ));

            // Act
            var result = await _controller.EditRole(roleDetails);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(response.Success);           
            Assert.Equal($"{role.Name} cannot be updated properly !", response.Message);
            
        }

        [Fact]
        public async Task EditRole_ReturnsJsonWithSuccessTrue_WhenUpdateAsyncSucceeds()
        {
            // Arrange
            RoleDetails roleDetails = Mock.Of<RoleDetails>();
            Role role = Mock.Of<Role>();
            _mockRoleManager.Setup(m => m.FindByIdAsync(roleDetails.ID)).ReturnsAsync(role);
            _mockRoleManager.Setup(m => m.UpdateAsync(role))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.EditRole(roleDetails);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"{role.Name} role is updated successfully !", response.Message);
        }

        [Fact]
        public async Task EditRoleAccess_ReturnsBadRequest_WhenRoleAccessListIsNullOrEmpty()
        {
            // Arrange
            List<RoleAccess>? accessList = null;

            // Act
            var result = await _controller.EditRoleAccess(accessList);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(DefaultValues.RoleAccessListUnavailableMessage, badResult.Value);
        }

        [Fact]
        public async Task EditRoleAccess_ReturnsNotFound_WhenUserInAccessListDoesntExist()
        {
            // Arrange
            List<RoleAccess> accessList = [Mock.Of<RoleAccess>()];
            _mockUserManager.Setup(m => m.FindByEmailAsync(accessList[0].UserID)).ReturnsAsync(null as User);

            // Act
            var result = await _controller.EditRoleAccess(accessList);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(DefaultValues.UserInAccessUnavailableMessage, notFoundResult.Value);
        }

        [Fact]
        public async Task EditRoleAccess_ReturnsJsonWithSuccessTrue_WhenRoleAccessIsUpdatedToUnauthorize()
        {
            // Arrange
            List<RoleAccess> accessList = [
                new RoleAccess() {UserID = "user1", Role = "Sudo", IsAuthorized = false}
            ];
            var user = Mock.Of<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(accessList[0].UserID)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, accessList[0].Role)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.RemoveFromRoleAsync(user, accessList[0].Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.EditRoleAccess(accessList);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Role access list for {accessList[0].Role} is updated !", response.Message);
        }

        [Fact]
        public async Task EditRoleAccess_ReturnsJsonWithSuccessTrue_WhenRoleAccessIsUpdatedToAuthorize_NoRoleRemovalNeeded()
        {
            // Arrange
            List<RoleAccess> accessList = [
                new RoleAccess() {UserID = "user1", Role = "Sudo", IsAuthorized = true}
            ];
            var user = Mock.Of<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(accessList[0].UserID)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, accessList[0].Role)).ReturnsAsync(false);
            _mockUserManager.Setup(m => m.AddToRoleAsync(user, accessList[0].Role)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Sudo"]);

            // Act
            var result = await _controller.EditRoleAccess(accessList);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Role access list for {accessList[0].Role} is updated !", response.Message);
        }
        [Fact]
        public async Task EditRoleAccess_ReturnsJsonWithSuccessTrue_WhenRoleAccessIsUpdatedToAuthorize_RoleRemovalNeeded()
        {
            // Arrange
            List<RoleAccess> accessList = [
                new RoleAccess() {UserID = "user1", Role = "Sudo", IsAuthorized = true}
            ];
            var user = Mock.Of<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(accessList[0].UserID)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, accessList[0].Role)).ReturnsAsync(false);
            _mockUserManager.Setup(m => m.AddToRoleAsync(user, accessList[0].Role)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(["Sudo", "Mudo"]);
            _mockUserManager.Setup(m => m.RemoveFromRoleAsync(user, "Mudo")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.EditRoleAccess(accessList);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var response = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(response.Success);
            Assert.Equal($"Role access list for {accessList[0].Role} is updated !", response.Message);
        }

    }
}