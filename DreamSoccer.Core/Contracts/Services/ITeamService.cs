﻿using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Dtos.Teams;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface ITeamService : IMessageService
    {
        Task<TeamInformationDto> GetMyTeamAsync(string email);
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync(SearchPlayerFilter input);

        Task<IEnumerable<TeamInformationDto>> GetAllTeams(SearchTeamFilter input);

        Task<bool> AddPlayerToMarketAsync(string owner, int playerId, long price);
    }
}