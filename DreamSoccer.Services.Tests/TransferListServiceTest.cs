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
    public class TransferListServiceTest
    {
        TransferListService service;
        IMapper mapper;
        Mock<IPlayerRepository> playerRepository;
        Mock<ITransferListRepository> transferListRepository;
        Mock<IUnitOfWork> unitOfWork;
        public TransferListServiceTest()
        {
            transferListRepository = new Mock<ITransferListRepository>();
            mapper = AutoMapperHelper.Create();
            playerRepository = new Mock<IPlayerRepository>();
            unitOfWork = new Mock<IUnitOfWork>();
            service = new TransferListService(mapper, playerRepository.Object,
                transferListRepository.Object, unitOfWork.Object);
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
                        TeamName ="Team1"
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
    }
}