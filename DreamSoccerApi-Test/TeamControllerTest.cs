using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccerApi.Controllers;
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
        public TeamControllerTest()
        {
            teamService = new Mock<ITeamService>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();
            controller = new TeamController(teamService.Object, httpContextAccessor.Object);
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
            var identity = new GenericIdentity(userId.ToString());
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed
            var context = new DefaultHttpContext()
            {
                User = contextUser
            };
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            var players = new List<PlayerDto>();
            players.Add(new PlayerDto()
            {
                FirstName = "Jhonatan",
                Age = 20,
                Position = DreamSoccer.Core.Entities.Enums.PositionEnum.Attackers,
                Country = "UK"
            });
            teamService.Setup(_ => _.GetMyTeamAsync(It.IsAny<string>())).Returns(Task.FromResult(players.AsEnumerable()));
            // Actual
            // Actual
            var actual = await controller.GetMyPlayersAsync();

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            httpContextAccessor.Verify(mock => mock.HttpContext, Times.Once());
            teamService.Verify(mock => mock.GetMyTeamAsync(It.IsAny<string>()), Times.Once());
        }


        #endregion


    }
}