using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Contracts.Services;
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
    public class TransfersControllerTest
    {
        TransfersController _controller;
        Mock<ITransferListService> _service;
        Mock<IHttpContextAccessor> _httpContextAccessor;
        IMapper mapper;
        public TransfersControllerTest()
        {
            _service = new Mock<ITransferListService>();
            mapper = AutoMapperHelper.Create();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _controller = new TransfersController(_service.Object, _httpContextAccessor.Object, mapper);
        }
        #region SearchPlayers
        [Fact]
        public void SearchPlayers_When_Access_By_Team_Owner()
        {
            // Arrange
            var nameMethod = nameof(_controller.GetSearchPlayerAsync);
            var methodInformation = _controller.GetType().GetMethod(nameMethod);

            // Actual
            var actualAttribute = methodInformation
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().ToArray();

            // Assert
            Assert.True(actualAttribute.Any());
            Assert.Equal("Team_Owner", actualAttribute[0].Roles);
        }

        [Theory]
        [InlineData("English", "Team1", "Jhonatan", 1000000, 5000000)]
        public async Task SearchPlayers_When_Data_Exist(string country, string teamName, string playerName, int minValue, int maxValue)
        {
            // Arrange
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
            _service.Setup(_ => _.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>())).Returns(Task.FromResult(players.AsEnumerable()));
            // Actual
            // Actual
            var actual = await _controller.GetSearchPlayerAsync(request);

            // Assert
            Assert.Equal(typeof(OkObjectResult), actual.GetType());
            _service.Verify(mock => mock.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }
        [Theory]
        [InlineData("English", "Team1", "Jhonatan", 1000000, 5000000)]
        public async Task SearchPlayers_When_Data_Not_Found(string country, string teamName, string playerName, int minValue, int maxValue)
        {
            // Arrange
            var request = new SearchPlayerRequest()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };
            var players = new List<PlayerDto>();
           
            _service.Setup(_ => _.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>())).Returns(Task.FromResult(players.AsEnumerable()));
            // Actual
            // Actual
            var actual = await _controller.GetSearchPlayerAsync(request);

            // Assert
            Assert.Equal(typeof(NotFoundObjectResult), actual.GetType());
            _service.Verify(mock => mock.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }

        [Theory]
        [InlineData("English", "Team1", "Jhonatan", 1000000, 5000000)]
        public async Task SearchPlayers_When_Data_Unhandled_Expection(string country, string teamName, string playerName, int minValue, int maxValue)
        {
            // Arrange
            var request = new SearchPlayerRequest()
            {
                Country = country,
                TeamName = teamName,
                PlayerName = playerName,
                MaxValue = maxValue,
                MinValue = minValue
            };
          
            _service.Setup(_ => _.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>())).Throws(new ArgumentException("Connection Timeout"));
            // Actual
            // Actual
            var actual = await _controller.GetSearchPlayerAsync(request);

            // Assert
            Assert.Equal(typeof(BadRequestObjectResult), actual.GetType());
            _service.Verify(mock => mock.SearchPlayerInMarketAsync(It.IsAny<SearchPlayerFilter>()), Times.Once());
        }

        #endregion

    }
}