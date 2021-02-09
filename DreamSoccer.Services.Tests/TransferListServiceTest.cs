using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;
using DreamSoccer.Repository.Implementations;
using DreamSoccer.Services.Test.Helpers;
using Moq;
using Xunit;

namespace DreamSoccerApi_Test
{
    public class TransferListServiceTest
    {
        TransferListService service;
        IMapper mapper;
        Mock<IPlayerRepository> playerRepository;
        Mock<ITransferListRepository> transferListRepository;
        Mock<IUserRepository> userRepository;
        Mock<IRandomRepository> randomRepository;
        Mock<ITeamRepository> teamRepository;
        Mock<ICurrentUserRepository> currentUserRepository;
        Mock<IUnitOfWork> unitOfWork;
        public TransferListServiceTest()
        {
            transferListRepository = new Mock<ITransferListRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            randomRepository = new Mock<IRandomRepository>();
            teamRepository = new Mock<ITeamRepository>();
            userRepository = new Mock<IUserRepository>();
            currentUserRepository = new Mock<ICurrentUserRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            service = new TransferListService(mapper, playerRepository.Object,
                transferListRepository.Object,
                unitOfWork.Object,
                teamRepository.Object,
                userRepository.Object,
                randomRepository.Object,
                currentUserRepository.Object);
        }
        #region SearchPlayerOnMarketList

        [Theory]
        [InlineData("English", "Team1", "Jhonatan", 1000000, 5000000)]
        public async Task SearchPlayerInMarketList_When_Success(string country, string teamName, string playerName, int minValue, int maxValue)
        {
            // Arrange
            var input = new SearchPlayerFilter()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };
            var players = new List<TransferList>();
            players.Add(new TransferList()
            {
                Player = new Player()
                {
                    FirstName = "Jhonatan",
                    Age = 20,
                    Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                    Country = "English",
                    Team = new Team()
                    {
                        TeamName = "Team1"
                    }
                },
                Value = 3000000
            });
            transferListRepository.Setup(mock => mock.SearchPlayerAsync(It.IsAny<SearchPlayerFilter>()))
                .Returns(Task.FromResult(players.AsQueryable()));

            // Actual
            var actual = await service.SearchPlayerInMarketAsync(input);

            // Assert
            transferListRepository.Verify(mock => mock.SearchPlayerAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
            Assert.True(actual.Any());
        }


        #endregion

        #region BuyPlayer

        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_Success(int transferId, string owner)
        {
            // Arrange
            var transferPlayer = new TransferList()
            {
                PlayerId = 1,
                Id = 2,
                Value = 2000000,
                Player = new Player()
                {
                    Id = 1,
                    FirstName = "Jhonatan",
                    LastName = "Christian",
                    TeamId = 1
                }

            };
            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(transferPlayer));


            var user = new User() { Email = "test1@email.com", TeamId = 2 };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            teamRepository.Setup(mock => mock.GetByIdAsync(It.Is<int>(n => n == 1)))
                 .Returns(Task.FromResult(new Team()
                 {
                     TeamName = "Team Source",
                     Id = 1,
                     Budget = 10000000
                 }));
            teamRepository.Setup(mock => mock.GetByIdAsync(It.Is<int>(n => n == 2)))
                .Returns(Task.FromResult(new Team()
                {
                    TeamName = "Team Destination",
                    Id = 2,
                    Budget = 10000000
                }));
            var increaseValue = new RandomRepository().GetRandomRatioForIncreaseValue();
            randomRepository.Setup(mock => mock.GetRandomRatioForIncreaseValue()).Returns(increaseValue);


            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Once());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Once());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Exactly(2));
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            Assert.NotNull(actual);
            Assert.Equal(8000000, actual.NextTeam.Budget);
            Assert.Equal(12000000, actual.PreviousTeam.Budget);
        }

        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_Same_Team(int transferId, string owner)
        {
            // Arrange
            var transferPlayer = new TransferList()
            {
                PlayerId = 1,
                Id = 2,
                Value = 2000000,
                Player = new Player()
                {
                    Id = 1,
                    FirstName = "Jhonatan",
                    LastName = "Christian",
                    TeamId = 1
                }

            };
            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(transferPlayer));


            var user = new User() { Email = "test1@email.com", TeamId = 1 };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            teamRepository.Setup(mock => mock.GetByIdAsync(It.Is<int>(n => n == 1)))
                 .Returns(Task.FromResult(new Team()
                 {
                     TeamName = "Team Source",
                     Id = 1,
                     Budget = 10000000
                 }));
            var increaseValue = new RandomRepository().GetRandomRatioForIncreaseValue();
            randomRepository.Setup(mock => mock.GetRandomRatioForIncreaseValue()).Returns(increaseValue);


            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Never());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            Assert.Null(actual);
            Assert.Equal("This player in your team", service.CurrentMessage);
        }
        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_Player_Not_Exist(int transferId, string owner)
        {
            // Arrange

            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult<TransferList>(null));


            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));




            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Never());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Never());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            Assert.Null(actual);
            Assert.Equal("Player not exists", service.CurrentMessage);
        }
        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_User_Not_Exist(int transferId, string owner)
        {
            // Arrange
            var transferPlayer = new TransferList()
            {
                PlayerId = 1,
                Id = 2,
                Value = 2000000,
                Player = new Player()
                {
                    Id = 1,
                    FirstName = "Jhonatan",
                    LastName = "Christian"
                }
            };
            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(transferPlayer));


            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<User>(null));




            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Never());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            Assert.Null(actual);
            Assert.Equal("User not exists", service.CurrentMessage);
        }
        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_Team_Not_Exist(int transferId, string owner)
        {
            // Arrange
            var transferPlayer = new TransferList()
            {
                PlayerId = 1,
                Id = 2,
                Value = 2000000,
                Player = new Player()
                {
                    Id = 1,
                    FirstName = "Jhonatan",
                    LastName = "Christian"
                }
            };
            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(transferPlayer));


            var user = new User() { Email = "test1@email.com" };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            teamRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<Team>(null));



            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Never());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            Assert.Null(actual);
            Assert.Equal("Team not exists", service.CurrentMessage);
        }

        [Theory]
        [InlineData(1, "test1@email.com")]
        public async Task BuyPlayer_When_Budget_Not_Enough(int transferId, string owner)
        {
            // Arrange
            var transferPlayer = new TransferList()
            {
                PlayerId = 1,
                Id = 2,
                Value = 2000000,
                Player = new Player()
                {
                    Id = 1,
                    FirstName = "Jhonatan",
                    LastName = "Christian"
                }
            };
            transferListRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(transferPlayer));


            var user = new User() { Email = "test1@email.com", TeamId = 2 };
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(user));
            var team = new Team()
            {
                TeamName = "Team Destination",
                Id = 2,
                Budget = 1000000
            };
            teamRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(team));


            // Actual
            var actual = await service.BuyPlayerAsync(transferId, owner);

            // Assert
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            transferListRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once);
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            teamRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            randomRepository.Verify(mock => mock.GetRandomRatioForIncreaseValue(), Times.Never());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            Assert.Null(actual);
            Assert.Equal("Budget not enough", service.CurrentMessage);
        }
        #endregion
    }
}