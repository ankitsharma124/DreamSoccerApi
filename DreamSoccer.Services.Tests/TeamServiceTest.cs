using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
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
        Mock<ITeamRepository> teamRepository;
        Mock<IUnitOfWork> unitOfWork;
        public TeamServiceTest()
        {
            userRepository = new Mock<IUserRepository>();
            transferListRepository = new Mock<ITransferListRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            teamRepository = new Mock<ITeamRepository>();
            service = new TeamService(mapper, userRepository.Object, playerRepository.Object,
                transferListRepository.Object, unitOfWork.Object, teamRepository.Object);
        }

        #region GetMyTeam

        [Theory]
        [InlineData("test1@email.com")]
        public async Task GetMyTeam_When_Return_Data(string email)
        {
            // Arrange
            var players = new List<Player>();
            players.Add(new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            var team = new Team()
            {
                Budget = 5000000,
                TeamName = "Team 1",
                Players = players
            };
            var user = new User() { Email = "test1@email.com", TeamId = 2, Team = team };
            userRepository.Setup(m =>
                  m.GetByEmailAsync(It.IsAny<string>())
                ).Returns(Task.FromResult(user));


            // Actual
            var actual = await service.GetMyTeamAsync(email);

            // Assert
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            Assert.NotNull(actual);
            Assert.True(actual.Players.Any());
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
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            Assert.Null(actual);
        }
        #endregion

        #region GetAllPlayers


        [Theory]
        [InlineData(250000)]
        public async Task GetAllPlayersAsync_When_Return_Data(long maxValue)
        {
            // Arrange
            var input = new SearchPlayerFilter
            {
                MaxValue = maxValue
            };
            var players = new List<Player>();
            players.Add(new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            playerRepository.Setup(m =>
                  m.SearchAsync(It.IsAny<SearchPlayerFilter>())
                ).Returns(Task.FromResult(players.AsQueryable()));

            // Actual
            var actual = await service.GetAllPlayersAsync(input);

            // Assert
            playerRepository.Verify(mock => mock.SearchAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
            Assert.True(actual.Any());
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
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Once());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Once());
            Assert.True(actual);
        }

        [Theory]
        [InlineData("test1@email.com", 2, 25000000)]
        public async Task AddPlayerToMarket_When_Player_Exists_In_Transfer_List(string owner, int playerId, long price)
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
            transferListRepository.Setup(m =>
               m.CheckPlayerExistAsync(It.IsAny<int>())
             ).Returns(Task.FromResult(true));


            // Actual
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Once());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            Assert.False(actual);
            Assert.Equal("Player Exist in transfer List", service.CurrentMessage);
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
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Never());
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
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Never());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Never());
            Assert.False(actual);
            Assert.Equal("User doesn't exist", service.CurrentMessage);
        }
        #endregion

        #region GetAllTeams


        [Theory]
        [InlineData("Team 1")]
        public async Task GetAllTeams_When_Return_Data(string teamName)
        {
            // Arrange
            var input = new SearchTeamFilter
            {
                TeamName = teamName
            };
            
            var players = new List<Player>();
            players.Add(new Player()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            var teams = new List<Team>()
            {
                new Team()
                {
                    TeamName ="Team 1",
                    Budget=5000000,
                    Country="US",
                    Players = players
                }
            };
            teamRepository.Setup(m =>
                  m.SearchTeams(It.IsAny<SearchTeamFilter>())
                ).Returns(Task.FromResult(teams.AsQueryable()));

            // Actual
            var actual = await service.GetAllTeams(input);

            // Assert
            teamRepository.Verify(mock => mock.SearchTeams(It.IsAny<SearchTeamFilter>()), Times.Once());
            Assert.True(actual.Any());
        }

        #endregion

    }
}