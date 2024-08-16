using AutoMapper;
using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Controllers;
using GarageCoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace GarageCoreAPI.Tests.Controllers
{
    public class AccountControllerTest
    {
        private readonly AccountController _controller;
        private readonly Mock<UserManager<User>> _mockUserManager;
        //private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IMapper> _mockMapper;

        public AccountControllerTest()
        {
            _mockUserManager = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            /* _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, Mock.Of<IHttpContextAccessor>(), 
                Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            */
            _mockMapper = new Mock<IMapper>();
            _controller = new AccountController(_mockMapper.Object, _mockUserManager.Object); // To test with sign-in manager, inject here !
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserIsNull()
        {
            // Arrange
            UserRegistration? rUser = null;

            // Act
            var result = await _controller.Register(rUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.Equal(DefaultValues.UserRegisterDtoUnavailableMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsJsonWithRegisteredFalse_WhenCreateAsyncFails()
        {
            // Arrange
            var rUser = new UserRegistration()
            {
                Name = "Dummy",
                Email = "dummy@dumbo.com",
                Password = "?Dummydumb0"
            };
            var user = new Mock<User>();
            _mockMapper.Setup(m => m.Map<User>(rUser)).Returns(user.Object);
            _mockUserManager.Setup(m => m.CreateAsync(user.Object, rUser.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = DefaultValues.DuplicateUserFailMessage }));

            // Act
            var result = await _controller.Register(rUser);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
            Assert.False(data.Registered);
            Assert.Equal(DefaultValues.DuplicateUserFailMessage, data.Message);
        }

        [Fact]
        public async Task Register_ReturnsJsonWithRegisteredTrue_WhenCreateAsyncSucceeds()
        {
            // Arrange
            var rUser = new UserRegistration()
            {
                Name = "Dummy",
                Email = "dummy@dumbo.com",
                Password = "?Dummydumb0"
            };
            var user = new Mock<User>();
            _mockMapper.Setup(m => m.Map<User>(rUser)).Returns(user.Object);
            _mockUserManager.Setup(m => m.CreateAsync(user.Object, rUser.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.AddToRoleAsync(user.Object, "Visitor"))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(rUser);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
            Assert.True(data.Registered);
            Assert.Equal($"User: {user.Object.Name} is successfully registered !!", data.Message);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserIsNull()
        {
            // Arrange
            UserLogin? lUser = null;

            // Act
            var result = await _controller.Login(lUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int) HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.Equal(DefaultValues.UserLoginDtoUnavailableMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsJsonWithLoggedInFalse_WhenFindByEmailAsyncReturnsNull()
        {
            // Arrange
            var lUser = new UserLogin()
            {
                Email = "dummy@dumbo.com",
                Password = "?Dummydumb0",
            };
            User? foundUser = null;
            _mockUserManager.Setup(m => m.FindByEmailAsync(lUser.Email)).ReturnsAsync(foundUser);

            // Act
            var result = await _controller.Login(lUser);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
            Assert.False(data.LoggedIn);
            Assert.Equal(DefaultValues.LoginFailedMessage, data.Message);
        }

        [Fact]
        public async Task Login_ReturnsJsonWithLoggedInTrue_WhenFindByEmailAsyncReturnsUserAndPasswordDoesnotMatch()
        {
            // Arrange
            var lUser = new UserLogin()
            {
                Email = "dummy@dumbo.com",
                Password = "?Dummydumb0",
                RememberMe = false
            };
            var user = new Mock<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(lUser.Email)).ReturnsAsync(user.Object);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user.Object, lUser.Password)).ReturnsAsync(false);

            // Act
            var result = await _controller.Login(lUser);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
            Assert.False(data.LoggedIn);
            Assert.Equal(DefaultValues.LoginFailedMessage, data.Message);
        }

        [Fact]
        public async Task Login_ReturnsJsonWithLoggedInTrue_WhenFindByEmailAsyncReturnsUserAndPasswordMatches()
        {
            // Arrange
            var lUser = new UserLogin()
            {
                Email = "dummy@dumbo.com",
                Password = "?Dummydumb0",
                RememberMe = false
            };
            var user = new User()
            {
                Id = "DUM100",
                UserName = "Dummy",
                Email = lUser.Email
            };
            _mockUserManager.Setup(m => m.FindByEmailAsync(lUser.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, lUser.Password)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync([]);

            // Act
            var result = await _controller.Login(lUser);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
            Assert.True(data.LoggedIn);
            Assert.Equal(data.Username, user.UserName);
            Assert.Equal(DefaultValues.LoginSuccessMessage, data.Message);
        }

        //[Fact]
        //public async Task Logout_ReturnsJsonWithLoggedInFalse_WhenSignOutSuceeds()
        //{
        //    // Act
        //    var result = await _controller.Logout();

        //    // Assert
        //    //_mockHttpContext.Verify(c => c.Session.Clear(), Times.Once);
        //    //_mockHttpResponse.Verify(r => r.Cookies.Delete(GlobalConfig.GeneratedTokenCookieName), Times.Once);
        //    var jsonResult = Assert.IsType<JsonResult>(result);
        //    var data = Assert.IsType<AuthResponseMessage>(jsonResult.Value);
        //    Assert.True(data.LoggedIn);
        //    Assert.Equal(DefaultValues.LoggedOutMessage, data.Message);
        //}

        [Fact]
        public async Task GetAllUsers_ReturnsJsonWithListofUserDtos()
        {
            // Arrange
            var users = new List<User>([
                new User(){ Name = "Hero", Email = "hero@hero.com", RememberMe = false },
            ]);
            _mockUserManager.Setup(m => m.Users).Returns(users.AsQueryable<User>);
            _mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(["Sudo"]);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<List<UserDto>>(jsonResult.Value);
            Assert.Single(data);
            Assert.Equal(data[0].Email, users[0].Email);
            Assert.Equal(data[0].Name, users[0].Name);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsJsonWithSuccessFalse_WhenFindByEmailAsyncReturnsNull()
        {
            // Arrange
            User? foundUser = null;
            _mockUserManager.Setup(m => m.FindByEmailAsync("Dummy")).ReturnsAsync(foundUser);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(data.Success);
            Assert.Equal((int) HttpStatusCode.BadRequest, data.StatusCode);
            Assert.Equal(DefaultValues.NoUserAuthenticatedYetMessage, data.Message);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsJsonWithUserDtos_WhenFindByEmailAsyncReturnsUser()
        {
            // Arrange
            var foundUser = new Mock<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(foundUser.Object);
            _mockUserManager.Setup(m => m.GetRolesAsync(foundUser.Object)).ReturnsAsync(["DummyRole"]);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.IsType<UserDto>(jsonResult.Value);
        }

        [Fact]
        public async Task ChangeUserPassword_ReturnsBadRequest_WhenChangeDtoIsNull()
        {
            // Arrange
            UserChangeDto? userChangeDto = null;

            // Act
            var result = await _controller.ChangeUserPassword(userChangeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.Equal(DefaultValues.PasswordChangeDtoUnavailableMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task ChangeUserPassword_ReturnsBadRequest_WhenFindByEmailAsyncFails()
        {
            // Arrange
            var userChangeDto = new Mock<UserChangeDto>();
            User? foundUser = null;
            _mockUserManager.Setup(m => m.FindByEmailAsync("Dummy")).ReturnsAsync(foundUser);

            // Act
            var result = await _controller.ChangeUserPassword(userChangeDto.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.Equal(DefaultValues.PasswordChangeRestrictedMessage, badRequestResult.Value);
        }

        [Fact]
        public async Task ChangeUserPassword_ReturnsJsonWithSuccessFalse_WhenCheckPasswordAsyncFails()
        {
            // Arrange
            var userChangeDto = new Mock<UserChangeDto>();
            var foundUser = new Mock<User>();
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(foundUser.Object);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(foundUser.Object, userChangeDto.Object.OldPassword)).ReturnsAsync(false);

            // Act
            var result = await _controller.ChangeUserPassword(userChangeDto.Object);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.False(data.Success);
            Assert.Equal(DefaultValues.UserPasswordWrongMessage, data.Message);
        }

        [Fact]
        public async Task ChangeUserPassword_ReturnsJsonWithSuccessTrue_WhenCheckPasswordAsyncSucceeds()
        {
            // Arrange
            var userChangeDto = new UserChangeDto()
            {
                OldPassword = "helloBrother#1", NewPassword = "helloSister#1"
            };
            var foundUser = new User()
            {
                Name = "Hero", Email = "hero@hero.com", RememberMe = false
            };
            _mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(foundUser);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(foundUser, userChangeDto.OldPassword)).ReturnsAsync(true);

            // Act
            var result = await _controller.ChangeUserPassword(userChangeDto);

            // Assert
            Assert.IsAssignableFrom<ActionResult>(result);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsType<ResponseMessage>(jsonResult.Value);
            Assert.True(data.Success);
            Assert.Equal($"Password for user: {foundUser.Name} is successfully changed !", data.Message);
        }
    }
}