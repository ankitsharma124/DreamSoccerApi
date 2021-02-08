using System.Linq;
using System.Threading.Tasks;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccerApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DreamSoccerApi_Test
{
    public class AuthControllerTest
    {
        AuthController controller;
        Mock<IUserService> userService;
        public AuthControllerTest()
        {
            userService = new Mock<IUserService>();
            controller = new AuthController(userService.Object);
        }
        #region Register
        [Fact]
        public void Register_When_Access_By_Anonymous_User()
        {
            // Arrange
            var nameMethod = nameof(controller.Register);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(!actualAttribute.Any());
        }

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Admin, "")]
        public async Task Register_When_Data_Not_Valid(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserRegisterDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            userRegistration.Password = password;
            controller.ModelState.AddModelError("RequiredField", "Missing Role and Password");
            // Actual
            var actual = await controller.Register(userRegistration);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            userService.Verify(mock => mock.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()), Times.Never());
        }

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Admin, "admin123")]
        public async Task Register_When_Data_Valid(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserRegisterDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            userRegistration.Password = password;
            userService.Setup(m =>
                    m.RegisterAsync(
                 It.IsAny<UserDto>(),
              It.IsAny<string>()
          )).Returns(Task.FromResult(new ServiceResponse<int>() { Success = true }));
            // Actual

            var actual = await controller.Register(userRegistration);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            userService.Verify(mock => mock.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()), Times.Once());
        }

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Admin, "admin123")]
        public async Task Register_When_Data_UnHandleException(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserRegisterDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            userRegistration.Password = password;
            userService.Setup(m =>
           m.RegisterAsync(
                It.IsAny<UserDto>(),
                It.IsAny<string>()
            )).Returns(Task.FromResult(new ServiceResponse<int>() { Success = false }));

            // Actual
            var actual = await controller.Register(userRegistration);

            // Assert

            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            userService.Verify(mock => mock.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()), Times.Once());
        }
        #endregion

        #region Login

        [Fact]
        public void Login_When_Access_By_Anonymous_User()
        {
            // Arrange
            var nameMethod = nameof(controller.Login);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(!actualAttribute.Any());
        }

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Admin, "")]
        public async Task Login_When_Data_Not_Valid(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserRegisterDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            userRegistration.Password = password;
            controller.ModelState.AddModelError("RequiredField", "Missing Role and Password");
            // Actual
            var actual = await controller.Register(userRegistration);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            userService.Verify(mock => mock.RegisterAsync(It.IsAny<UserDto>(), It.IsAny<string>()), Times.Never());
        }

        [Theory]
        [InlineData("test1@email.com", "admin123")]
        public async Task Login_When_Data_Valid(string email, string password)
        {
            // Arrange
            var userLogin = new UserLoginDto();
            userLogin.Email = email;
            userLogin.Password = password;
            userService.Setup(m =>
                    m.LoginAsync(
                 It.IsAny<string>(),
              It.IsAny<string>()
          )).Returns(Task.FromResult(new ServiceResponse<string>() { Success = true }));
            // Actual

            var actual = await controller.Login(userLogin);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            userService.Verify(mock => mock.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Theory]
        [InlineData("test1@email.com", "admin123")]
        public async Task Login_When_Data_UnHandleException(string email, string password)
        {
            // Arrange
            var userLogin = new UserLoginDto();
            userLogin.Email = email;
            userLogin.Password = password;
            userService.Setup(m =>
           m.LoginAsync(
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns(Task.FromResult(new ServiceResponse<string>() { Success = false }));

            // Actual
            var actual = await controller.Login(userLogin);
            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            userService.Verify(mock => mock.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        #endregion
    }
}