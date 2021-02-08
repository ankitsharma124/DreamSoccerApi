using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<PlayerDto>> GetMyTeamAsync(string email);
        Task<bool> AddPlayerToMarket(string owner, int playerId, long price);
    }
}