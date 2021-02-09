using System;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Repository.Context;
using DreamSoccer.Repository.Implementations;
using DreamSoccer.Services.Test.Helpers;
using Moq;
using Xunit;

namespace DreamSoccerApi_Test
{
    public class UserServiceTest
    {
        UserService service;
        Mock<IAuthRepository> authRepository;
        IMapper mapper;
        Mock<ITeamRepository> teamRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<IRandomRepository> randomRepository;
        public UserServiceTest()
        {
            authRepository = new Mock<IAuthRepository>();
            mapper = AutoMapperHelper.Create();
            teamRepository = new Mock<ITeamRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            randomRepository = new Mock<IRandomRepository>();
            service = new UserService(authRepository.Object,
                mapper,
                teamRepository.Object,
                unitOfWork.Object,
                randomRepository.Object);
        }
        #region Register

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Admin, "")]
        public async Task Register_When_Data_Have_UnHandleExceptions(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            authRepository.Setup(m =>
                  m.RegisterAsync(
               It.IsAny<User>(),
            It.IsAny<string>()
                )).Throws(new ArgumentException("User Already Exist"));

            // Actual
            var actual = await service.RegisterAsync(userRegistration, password);

            // Assert
            authRepository.Verify(mock => mock.RegisterAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
            randomRepository.Verify(mock => mock.GetRandomPlayer(), Times.Never());
            randomRepository.Verify(mock => mock.GetRandomTeam(), Times.Never());
            teamRepository.Verify(mock => mock.CreateAsync(It.IsAny<Team>()), Times.Never());
            Assert.False(actual.Success);
            Assert.Equal("User Already Exist", actual.Message);
        }

        [Theory]
        [InlineData("test1@email.com", RoleEnum.Team_Owner, "")]
        public async Task Register_When_Data_Valid(string email, RoleEnum role, string password)
        {
            // Arrange
            var userRegistration = new UserDto();
            userRegistration.Email = email;
            userRegistration.Role = role;
            authRepository.Setup(m =>
                  m.RegisterAsync(
               It.IsAny<User>(),
            It.IsAny<string>()
        )).Returns(Task.FromResult(new ServiceResponse<int>() { Success = true }));


            randomRepository.Setup(m =>
                  m.GetRandomPlayer()
                ).Returns(new RandomRepository().GetRandomPlayer());
            randomRepository.Setup(m =>
                  m.GetRandomTeam()
                ).Returns(new RandomRepository().GetRandomTeam());

            // Actual
            var actual = await service.RegisterAsync(userRegistration, password);

            // Assert
            authRepository.Verify(mock => mock.RegisterAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once());
            randomRepository.Verify(mock => mock.GetRandomPlayer(), Times.Exactly(20));
            randomRepository.Verify(mock => mock.GetRandomTeam(), Times.Once());
            teamRepository.Verify(mock => mock.CreateAsync(It.IsAny<Team>()), Times.Once());
            Assert.True(actual.Success);
        }

        #endregion

        #region Login

        [Theory]
        [InlineData("test1@email.com", "")]
        public async Task Login_When_Data_Have_UnHandleExceptions(string email, string password)
        {
            // Arrange
            authRepository.Setup(m =>
                  m.LoginAsync(
               It.IsAny<string>(),
            It.IsAny<string>()
                )).Returns(Task.FromResult(new ServiceResponse<string>() { Success = false, Message = "User Not Found" }));

            // Actual
            var actual = await service.LoginAsync(email, password);

            // Assert
            authRepository.Verify(mock => mock.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.False(actual.Success);
            Assert.Equal("User Not Found", actual.Message);
        }

        [Theory]
        [InlineData("test1@email.com", "password")]
        public async Task Login_When_Data_Success(string email, string password)
        {
            // Arrange
            authRepository.Setup(m =>
                  m.LoginAsync(
               It.IsAny<string>(),
            It.IsAny<string>()
                )).Returns(Task.FromResult(new ServiceResponse<string>() { Success = true }));

            // Actual
            var actual = await service.LoginAsync(email, password);

            // Assert
            authRepository.Verify(mock => mock.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.True(actual.Success);
            Assert.Null(actual.Message);
        }

        #endregion 

        #region UserExist

        [Theory]
        [InlineData("test1@email.com")]
        public async Task UserExist_When_Data_Have_UnHandleExceptions(string email)
        {
            // Arrange
            authRepository.Setup(m =>
                  m.UserExistAsync(
               It.IsAny<string>()
                )).Returns(Task.FromResult(false));

            // Actual
            var actual = await service.UserExistAsync(email);

            // Assert
            authRepository.Verify(mock => mock.UserExistAsync(It.IsAny<string>()), Times.Once());
            Assert.False(actual);
        }

        [Theory]
        [InlineData("test1@email.com")]
        public async Task UserExist_When_Data_Exist(string email)
        {
            // Arrange
            authRepository.Setup(m =>
                  m.UserExistAsync(
               It.IsAny<string>()
                )).Returns(Task.FromResult(true));

            // Actual
            var actual = await service.UserExistAsync(email);

            // Assert
            authRepository.Verify(mock => mock.UserExistAsync(It.IsAny<string>()), Times.Once());
            Assert.True(actual);
        }

        #endregion 
    }
}