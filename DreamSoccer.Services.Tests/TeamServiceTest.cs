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
using DreamSoccer.Services.Test.Helpers;
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
        Mock<ITransferListRepository> transferListRepository;
        Mock<IUnitOfWork> unitOfWork;
        public TeamServiceTest()
        {
            userRepository = new Mock<IUserRepository>();
            transferListRepository = new Mock<ITransferListRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            service = new TeamService(mapper, userRepository.Object, playerRepository.Object,
                transferListRepository.Object, unitOfWork.Object);
        }

        #region GetMyTeam


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
        #endregion

        #region AddPlayerToMarket

        [Theory]
        [InlineData("test1@email.com", 2, 25000000)]
        public async Task AddPlayerToMarket_When_Success(string owner, int playerId, long price)
        {
            // Arrange
            var user = new User() { Email = "test1@email.com", TeamId = 2 };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            var player = new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK",
                Team = new Team()
                {
                    TeamName = "Team 1",
                    Owner = user
                }
            };
            playerRepository.Setup(m =>
                  m.GetByIdAsync(It.IsAny<int>())
                ).Returns(Task.FromResult(player));

            // Actual
            var actual = await service.AddPlayerToMarket(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Once());
            Assert.True(actual);
        }

        [Theory]
        [InlineData("test1@email.com", 2, 25000000)]
        public async Task AddPlayerToMarket_When_Palyer_Not_In_Our_Team(string owner, int playerId, long price)
        {
            // Arrange
            var user = new User() { Email = "test1@email.com", TeamId = 2 };
            var otherUser = new User() { Email = "test2@email.com", TeamId = 3 };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            var player = new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK",
                Team = new Team()
                {
                    TeamName = "Team 1",
                    Owner = otherUser
                }
            };
            playerRepository.Setup(m =>
                  m.GetByIdAsync(It.IsAny<int>())
                ).Returns(Task.FromResult(player));

            // Actual
            var actual = await service.AddPlayerToMarket(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            Assert.False(actual);
            Assert.Equal("Player not in our Team", service.CurrentMessage);
        }


        [Theory]
        [InlineData("test1@email.com", 2, 25000000)]
        public async Task AddPlayerToMarket_When_User_Not_Exist(string owner, int playerId, long price)
        {
            // Arrange
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));
            
            // Actual
            var actual = await service.AddPlayerToMarket(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            Assert.False(actual);
            Assert.Equal("User doesn't exist",service.CurrentMessage);
        }
        #endregion

    }
}