using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Dtos.Teams;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Requests;
using DreamSoccer.Core.Responses;
using DreamSoccerApi.Controllers;
using DreamSoccerApi_Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DreamSoccerApi_Test
{
    public class TeamControllerTest
    {
        TeamController controller;
        Mock<ITeamService> teamService;
        Mock<IHttpContextAccessor> httpContextAccessor;
        IMapper mapper;
        public TeamControllerTest()
        {
            teamService = new Mock<ITeamService>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            mapper = AutoMapperHelper.Create();
            controller = new TeamController(teamService.Object, httpContextAccessor.Object, mapper);
        }
        #region GetMyMembers
        [Fact]
        public void GetMyMembers_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(controller.GetMyPlayersAsync);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData(2)]
        public async Task GetMyMembers_When_User_Exist(int userId)
        {
            // Arrange

            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var team = new TeamInformationDto();
            team.TeamName = "Team 1";
            team.Budget = 52000000;
            var players = new List<PlayersInformationDto>();
            team.Players = players;
            players.Add(new PlayersInformationDto()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK",
                Value = 100000
            });

            teamService.Setup(_ => _.GetMyTeamAsync(It.IsAny<string>())).Returns(Task.FromResult(team));
            // Actual
            // Actual
            var actual = await controller.GetMyPlayersAsync();

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Once());
            teamService.Verify(mock => mock.GetMyTeamAsync(It.IsAny<string>()), Times.Once());
        }


        [Theory]
        [InlineData(2)]
        public async Task GetMyMembers_When_No_Data(int userId)
        {
            // Arrange

            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var players = new List<PlayerDto>();
            teamService.Setup(_ => _.GetMyTeamAsync(It.IsAny<string>())).Returns(Task.FromResult<TeamInformationDto>(null));
            // Actual
            // Actual
            var actual = await controller.GetMyPlayersAsync();

            // Assert
            Assert.Equal(typeof(NotFoundObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Once());
            teamService.Verify(mock => mock.GetMyTeamAsync(It.IsAny<string>()), Times.Once());
        }

        [Theory]
        [InlineData(2)]
        public async Task GetMyMembers_When_Un_Handle_Exceptions(int userId)
        {
            // Arrange

            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var players = new List<PlayerDto>();
            teamService.Setup(_ => _.GetMyTeamAsync(It.IsAny<string>())).Throws(new ArgumentException("Connection Time out"));
            // Actual
            // Actual
            var actual = await controller.GetMyPlayersAsync();

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Once());
            teamService.Verify(mock => mock.GetMyTeamAsync(It.IsAny<string>()), Times.Once());
        }

        #endregion

        #region AddPlayerToMarket
        [Fact]
        public void AddPlayerToMarket_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(controller.AddPlayerToMarketAsycn);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner,Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task AddPlayerToMarket_When_Successt(int userId, int playerId, long price)
        {
            // Arrange
            var request = new AddTransferListRequest()
            {
                Price = price,
                PlayerId = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            teamService.Setup(_ => _.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>())).Returns(Task.FromResult(true));
            // Actual
            // Actual
            var actual = await controller.AddPlayerToMarketAsycn(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task AddPlayerToMarket_When_Failed(int userId, int playerId, long price)
        {
            // Arrange
            var request = new AddTransferListRequest()
            {
                Price = price,
                PlayerId = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            teamService.Setup(_ => _.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>())).Returns(Task.FromResult(false));
            // Actual
            // Actual
            var actual = await controller.AddPlayerToMarketAsycn(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task AddPlayerToMarket_When_UnhandleException(int userId, int playerId, long price)
        {
            // Arrange
            var request = new AddTransferListRequest()
            {
                Price = price,
                PlayerId = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            teamService.Setup(_ => _.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await controller.AddPlayerToMarketAsycn(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.AddPlayerToMarketAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<long>()), Times.Once());
        }


        #endregion

        #region GetAllPlayers
        [Fact]
        public void GetAllPlayers_When_Access_By_Admin()
        {
            // Arrange
            var nameMethod = nameof(controller.GetAllPlayers);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData("UK", "Team1", "Jhonatan", 1000000, 5000000, 2)]
        public async Task GetAllPlayers_When_Data_Exist(string country, string teamName, string playerName, int minValue, int maxValue, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchPlayerRequest()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };
            var players = new List<PlayerDto>();
            players.Add(new PlayerDto()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            teamService.Setup(_ => _.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>())).Returns(Task.FromResult(players.AsEnumerable()));
            // Actual
            // Actual
            var actual = await controller.GetAllPlayers(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }
        [Theory]
        [InlineData("UK", "Team1", "Jhonatan", 1000000, 5000000, 2)]
        public async Task SearchPlayers_When_Data_Not_Found(string country, string teamName, string playerName, int minValue, int maxValue, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchPlayerRequest()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };
            var players = new List<PlayerDto>();

            teamService.Setup(_ => _.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>())).Returns(Task.FromResult(players.AsEnumerable()));
            // Actual
            // Actual
            var actual = await controller.GetAllPlayers(request);

            // Assert
            Assert.Equal(typeof(NotFoundObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }

        [Theory]
        [InlineData("UK", "Team1", "Jhonatan", 1000000, 5000000, 2)]
        public async Task SearchPlayers_When_Data_Unhandled_Expection(string country, string teamName, string playerName, int minValue, int maxValue, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchPlayerRequest()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };

            teamService.Setup(_ => _.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await controller.GetAllPlayers(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllPlayersAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }

        #endregion

        #region GetAllTeams
        [Fact]
        public void GetAllTeams_When_Access_By_Admin()
        {
            // Arrange
            var nameMethod = nameof(controller.GetAllTeams);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData("UK", "Team1", 2)]
        public async Task GetAllTeams_When_Data_Exist(string country, string teamName, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchTeamRequest()
            {
                Country = country,
                TeamName = teamName,
            };

            var players = new List<PlayersInformationDto>();
            players.Add(new PlayersInformationDto()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            var teams = new List<TeamInformationDto>() {
                new TeamInformationDto()
                {
                    TeamName ="Team1",
                    Country="UK",
                    Players = players
                }
            };
            teamService.Setup(_ => _.GetAllTeams(It.IsAny<SearchTeamFilter>())).Returns(Task.FromResult(teams.AsEnumerable()));
            // Actual
            // Actual
            var actual = await controller.GetAllTeams(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllTeams(It.IsAny<SearchTeamFilter>()), Times.Once());
        }
        [Theory]
        [InlineData("UK", "Team1", 2)]
        public async Task GetAllTeams_When_Data_Not_Found(string country, string teamName, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchTeamRequest()
            {
                Country = country,
                TeamName = teamName,
            };


            var teams = new List<TeamInformationDto>();
            teamService.Setup(_ => _.GetAllTeams(It.IsAny<SearchTeamFilter>())).Returns(Task.FromResult(teams.AsEnumerable()));
            // Actual
            // Actual
            var actual = await controller.GetAllTeams(request);

            // Assert
            Assert.Equal(typeof(NotFoundObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllTeams(It.IsAny<SearchTeamFilter>()), Times.Once());
        }

        [Theory]
        [InlineData("UK", "Team1", 2)]
        public async Task GetAllTeams_When_Data_Unhandled_Expection(string country, string teamName, int userId)
        {
            // Arrange
            httpContextAccessor = AuthorizationHelper.CreateUserLogin(httpContextAccessor, userId);
            var request = new SearchTeamRequest()
            {
                Country = country,
                TeamName = teamName,
            };


            var teams = new List<TeamInformationDto>();
            teamService.Setup(_ => _.GetAllTeams(It.IsAny<SearchTeamFilter>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await controller.GetAllTeams(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            teamService.Verify(mock => mock.GetAllTeams(It.IsAny<SearchTeamFilter>()), Times.Once());
        }

        #endregion

        #region UpdatePlayer
        [Fact]
        public void UpdatePlayer_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(controller.UpdatePlayerAsync);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner,Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task UpdatePlayer_When_Successt(int userId, int playerId, long price)
        {
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.UpdatePlayerAsync(It.IsAny<PlayerDto>())).Returns(Task.FromResult(player));
            // Actual
            // Actual
            var actual = await controller.UpdatePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdatePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task UpdatePlayer_When_Failed(int userId, int playerId, long price)
        {
            // Arrange
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.UpdatePlayerAsync(It.IsAny<PlayerDto>())).Returns(Task.FromResult<PlayerDto>(null));
            // Actual
            // Actual
            var actual = await controller.UpdatePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdatePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task UpdatePlayer_When_UnhandleException(int userId, int playerId, long price)
        {
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.UpdatePlayerAsync(It.IsAny<PlayerDto>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await controller.UpdatePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdatePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }


        #endregion

        #region DeletePlayer
        [Fact]
        public void DeletePlayer_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(controller.DeletePlayerAsync);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner,Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task DeletePlayer_When_Successt(int userId, int playerId, long price)
        {
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.DeletePlayerAsync(It.IsAny<PlayerDto>())).Returns(Task.FromResult(player));
            // Actual
            // Actual
            var actual = await controller.DeletePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.DeletePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task DeletePlayer_When_Failed(int userId, int playerId, long price)
        {
            // Arrange
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.DeletePlayerAsync(It.IsAny<PlayerDto>())).Returns(Task.FromResult<PlayerDto>(null));
            // Actual
            // Actual
            var actual = await controller.DeletePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.DeletePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 2500000)]
        public async Task DeletePlayer_When_UnhandleException(int userId, int playerId, long price)
        {
            // Arrange
            var request = new PlayerReqeust()
            {
                Value = price,
                Id = playerId
            };
            httpContextAccessor.CreateUserLogin(userId);
            var player = new PlayerDto()
            {
                Id = playerId,
                Value = price
            };
            teamService.Setup(_ => _.DeletePlayerAsync(It.IsAny<PlayerDto>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await controller.DeletePlayerAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.DeletePlayerAsync(It.IsAny<PlayerDto>()), Times.Once());
        }


        #endregion

        #region UpdateTeam
        [Fact]
        public void UpdateTeam_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(controller.UpdateTeamAsync);
            var methodInformation = controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner,Admin", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData(2, 1, 50000000, "Team1", "US")]
        public async Task UpdateTeam_When_Successt(int userId, int teamId, long budget, string teamName, string country)
        {
            // Arrange
            var request = new TeamReqeust()
            {

                Id = teamId,
                Budget = budget,
                TeamName = teamName,
                Country = country
            };
            httpContextAccessor.CreateUserLogin(userId);
            var team = new TeamDto()
            {
                Id = teamId,
                Budget = budget
            };
            teamService.Setup(_ => _.UpdateTeamAsync(It.IsAny<TeamDto>())).Returns(Task.FromResult(team));
            // Actual
            // Actual
            var actual = await controller.UpdateTeamAsync(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdateTeamAsync(It.IsAny<TeamDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 50000000, "Team1", "US")]
        public async Task UpdateTeam_When_Failed(int userId, int teamId, long budget, string teamName, string country)
        {
            // Arrange
            var request = new TeamReqeust()
            {

                Id = teamId,
                Budget = budget,
                TeamName = teamName,
                Country = country
            };
            httpContextAccessor.CreateUserLogin(userId);
           
            teamService.Setup(_ => _.UpdateTeamAsync(It.IsAny<TeamDto>())).Returns(Task.FromResult<TeamDto>(null));
            // Actual
            // Actual
            var actual = await controller.UpdateTeamAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdateTeamAsync(It.IsAny<TeamDto>()), Times.Once());
        }

        [Theory]
        [InlineData(2, 1, 50000000, "Team1", "US")]
        public async Task UpdateTeam_When_UnHandleException(int userId, int teamId, long budget, string teamName, string country)
        {
            // Arrange
            var request = new TeamReqeust()
            {

                Id = teamId,
                Budget = budget,
                TeamName = teamName,
                Country = country
            };
            httpContextAccessor.CreateUserLogin(userId);

            teamService.Setup(_ => _.UpdateTeamAsync(It.IsAny<TeamDto>())).Throws(new ArgumentException("Connection Time out"));
            // Actual
            // Actual
            var actual = await controller.UpdateTeamAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Exactly(1));
            teamService.Verify(mock => mock.UpdateTeamAsync(It.IsAny<TeamDto>()), Times.Once());
        }



        #endregion
    }
}