using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Repository.Context;
using Moq;
using Xunit;

namespace DreamSoccerApi_Test
{
    public class TeamServiceTest
    {
        TeamService service;
        Mock<IUserRepository> userRepository;
        IMapper mapper;
        Mock<IPlayerRepository> playerRepository;
        public TeamServiceTest()
        {
            userRepository = new Mock<IUserRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            service = new TeamService(mapper, userRepository.Object, playerRepository.Object);
        }

        [Theory]
        [InlineData("test1@email.com")]
        public async Task GetMyTeam_When_Return_Data(string email)
        {
            // Arrange
            var users = new List<User>();
            users.Add(new User() { Email = "test1@email.com", TeamId = 2 });
            userRepository.Setup(m =>
                  m.GetAllAsync()
                ).Returns(Task.FromResult(users.AsQueryable()));

            var players = new List<Player>();
            players.Add(new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            playerRepository.Setup(m =>
                  m.GetPlayerByTeamIdAsync(It.IsAny<int>())
                ).Returns(Task.FromResult(players.AsQueryable()));

            // Actual
            var actual = await service.GetMyTeamAsync(email);

            // Assert
            userRepository.Verify(mock => mock.GetAllAsync(), Times.Once());
            playerRepository.Verify(mock => mock.GetPlayerByTeamIdAsync(It.IsAny<int>()), Times.Once());
            Assert.True(actual.Any());
        }

        [Theory]
        [InlineData("test1@email.com")]
        public async Task GetMyTeam_When_User_NotExist(string email)
        {
            // Arrange
            var users = new List<User>();
            userRepository.Setup(m =>
                  m.GetAllAsync()
                ).Returns(Task.FromResult(users.AsQueryable()));

            // Actual
            var actual = await service.GetMyTeamAsync(email);

            // Assert
            userRepository.Verify(mock => mock.GetAllAsync(), Times.Once());
            playerRepository.Verify(mock => mock.GetPlayerByTeamIdAsync(It.IsAny<int>()), Times.Never());
            Assert.False(actual.Any());
        }

    }
}