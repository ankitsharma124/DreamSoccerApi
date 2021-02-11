using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.Players;
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
        Mock<ICurrentUserRepository> currentUserRepository;
        Mock<IUnitOfWork> unitOfWork;
        public TeamServiceTest()
        {
            userRepository = new Mock<IUserRepository>();
            transferListRepository = new Mock<ITransferListRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            currentUserRepository = new Mock<ICurrentUserRepository>();
            teamRepository = new Mock<ITeamRepository>();
            service = new TeamService(mapper, userRepository.Object, playerRepository.Object,
                transferListRepository.Object, unitOfWork.Object, teamRepository.Object, currentUserRepository.Object);
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
                TeamId = 1,
                Team = new Team()
                {
                    TeamName = "Team 1",
                    Owner = user
                }
            };
            playerRepository.Setup(m =>
                  m.GetByIdAsync(It.IsAny<int>())
                ).Returns(Task.FromResult(player));
            var transferList = new TransferList();
            transferList.Id = 1;
            transferListRepository.Setup(m =>
                  m.CreateAsync(It.IsAny<TransferList>())
                ).Returns(Task.FromResult(transferList));

            // Actual
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Once());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Once());
            Assert.True(actual > 0);
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
            };
            playerRepository.Setup(m =>
                  m.GetByIdAsync(It.IsAny<int>())
                ).Returns(Task.FromResult(player));
            transferListRepository.Setup(m =>
               m.CheckPlayerExistAsync(It.IsAny<int>())
             ).Returns(Task.FromResult(new TransferList()));


            // Actual
            var actual = await service.AddPlayerToMarketAsync(owner, playerId, price);

            // Assert
            playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Once());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            Assert.False(actual > 0);
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
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Once());
            Assert.False(actual > 0);
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
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            transferListRepository.Verify(mock => mock.CreateAsync(It.IsAny<TransferList>()), Times.Never());
            transferListRepository.Verify(mock => mock.CheckPlayerExistAsync(It.IsAny<int>()), Times.Never());
            Assert.False(actual > 0);
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


        #region UpdateTeamAsync

        [Theory]
        [InlineData(1, "Team 1", "US", 5000000, RoleEnum.Admin)]
        [InlineData(1, "Team 2", "US", 5200000, RoleEnum.Team_Owner)]
        [InlineData(1, "Team 2", "US", 5400000, RoleEnum.Team_Owner)]
        public async Task UpdateTeam_When_Return_Data(int teamId, string teamName, string country, long budget, RoleEnum role)
        {
            // Arrange
            int currentBudget = 5200000;
            var input = new TeamDto
            {
                Id = teamId,
                TeamName = teamName,
                Country = country,
                Budget = budget
            };
            var team = new Team
            {
                Id = teamId,
                TeamName = teamName,
                Country = country,
                Budget = budget
            };
            teamRepository.Setup(mock => mock.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(team));
            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = teamId,
                Team = new Team()
                {
                    Budget = currentBudget
                }
            }));
            teamRepository.Setup(m =>
                  m.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>())
                );

            // Actual
            var actual = await service.UpdateTeamAsync(input);

            // Assert

            if (role == RoleEnum.Admin)
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Never());
                teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Once());
                unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            }
            else
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
                if (currentBudget != budget)
                {
                    Assert.Equal("You can't change budget", service.CurrentMessage);
                    unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
                    Assert.Null(actual);
                    teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
                }
                else
                {
                    teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Once());
                    unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
                }
            }

        }
        [Theory]
        [InlineData(1, "Team 1", "US", 5000000, RoleEnum.Team_Owner)]
        public async Task UpdateTeam_When_Return_Not_Have_Access(int teamId, string teamName, string country, long budget, RoleEnum role)
        {
            // Arrange
            var input = new TeamDto
            {
                Id = teamId,
                TeamName = teamName,
                Country = country,
                Budget = budget
            };

            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = 2
            }));
            teamRepository.Setup(m =>
                  m.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>())
                );

            // Actual
            var actual = await service.UpdateTeamAsync(input);

            // Assert
            teamRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Team>()), Times.Never());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            Assert.Null(actual);
            Assert.Equal("It's not your team", service.CurrentMessage);
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());

        }


        #endregion

        #region UpdatePlayer

        [Theory]
        [InlineData(1, "Jhonatan", "Chirstian", 200000, RoleEnum.Admin)]
        [InlineData(1, "Jhonatan", "Chirstian", 200000, RoleEnum.Team_Owner)]
        [InlineData(1, "Team 2", "US", 5200000, RoleEnum.Team_Owner)]
        public async Task UpdatePlayer_When_Return_Data(int playerId, string firstName, string lastName, long value, RoleEnum role)
        {
            // Arrange
            int currentValue = 200000;
            var input = new PlayerDto
            {
                Id = playerId,
                FirstName = firstName,
                LastName = lastName,
                Value = value,
                TeamId = 2

            };
            playerRepository.Setup(n => n.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(new Player()
            {
                Value = 200000,
                TeamId = 2
            }));
            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = 2
            }));
            playerRepository.Setup(m =>
                  m.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>())
                );

            // Actual
            var actual = await service.UpdatePlayerAsync(input);

            // Assert
            if (role == RoleEnum.Admin)
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Never());
                playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Once());
                unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
            }
            else
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
                if (currentValue != value)
                {
                    Assert.Equal("You can't change value", service.CurrentMessage);
                    Assert.Null(actual);
                    playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
                    playerRepository.Verify(mock => mock.GetByIdAsync(It.IsAny<int>()), Times.Once());
                    unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
                }
                else
                {
                    playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Once());
                    unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());
                }
            }

        }

        [Theory]
        [InlineData(1, "Team 2", "US", 5200000, RoleEnum.Team_Owner)]
        public async Task UpdatePlayer_When_Not_Have_Access(int playerId, string firstName, string lastName, long value, RoleEnum role)
        {
            // Arrange
            var input = new PlayerDto
            {
                Id = playerId,
                FirstName = firstName,
                LastName = lastName,
                Value = value,
                TeamId = 2

            };
            var player = new Player
            {
                Id = playerId,
                FirstName = firstName,
                LastName = lastName,
                Value = value,
                TeamId = 2

            };
            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = 1
            }));
            playerRepository.Setup(m =>
                 m.GetByIdAsync(It.IsAny<int>())
               ).Returns(Task.FromResult(player));
            playerRepository.Setup(m =>
                  m.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>())
                );

            // Actual
            var actual = await service.UpdatePlayerAsync(input);

            // Assert
            playerRepository.Verify(mock => mock.UpdateAsync(It.IsAny<int>(), It.IsAny<Player>()), Times.Never());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            Assert.Null(actual);
            Assert.Equal("Player not in your team", service.CurrentMessage);
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
        }


        #endregion

        #region DeletePlayer

        [Theory]
        [InlineData(1, "Jhonatan", "Chirstian", 200000, RoleEnum.Admin)]
        [InlineData(1, "Jhonatan", "Chirstian", 200000, RoleEnum.Team_Owner)]
        [InlineData(1, "Team 2", "US", 5200000, RoleEnum.Team_Owner)]
        public async Task DeletePlayer_When_Success(int playerId, string firstName, string lastName, long value, RoleEnum role)
        {
            // Arrange

            var input = new PlayerDto
            {
                Id = playerId,
                FirstName = firstName,
                LastName = lastName,
                Value = value,
                TeamId = 2

            };
            playerRepository.Setup(n => n.DeleteAsync(It.IsAny<int>())).Returns(Task.FromResult(new Player()
            {
                Value = 200000
            }));
            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = 2
            }));
            playerRepository.Setup(m =>
                  m.DeleteAsync(It.IsAny<int>())
                );

            // Actual
            var actual = await service.DeletePlayerAsync(input);

            // Assert
            if (role == RoleEnum.Admin)
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Never());
                playerRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once());
            }
            else
            {
                userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
                playerRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Once());
            }
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Once());

        }

        [Theory]
        [InlineData(1, "Team 2", "US", 5200000, RoleEnum.Team_Owner)]
        public async Task DeletePlayer_When_Not_Have_Access(int playerId, string firstName, string lastName, long value, RoleEnum role)
        {
            // Arrange
            var input = new PlayerDto
            {
                Id = playerId,
                FirstName = firstName,
                LastName = lastName,
                Value = value,
                TeamId = 2

            };

            currentUserRepository.Setup(n => n.Role).Returns(role);
            userRepository.Setup(mock => mock.GetByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(new User()
            {
                Id = 1,
                Role = role,
                TeamId = 1
            }));
            playerRepository.Setup(m =>
                  m.DeleteAsync(It.IsAny<int>())
                );

            // Actual
            var actual = await service.DeletePlayerAsync(input);

            // Assert
            playerRepository.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never());
            userRepository.Verify(mock => mock.GetByEmailAsync(It.IsAny<string>()), Times.Once());
            unitOfWork.Verify(mock => mock.SaveChangesAsync(), Times.Never());
            Assert.Null(actual);
            Assert.Equal("Player not in your team", service.CurrentMessage);
        }


        #endregion
    }
}