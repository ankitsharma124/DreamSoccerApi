using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface ITeamService : IMessageService
    {
        Task<IEnumerable<PlayerDto>> GetMyTeamAsync(string email);
        Task<bool> AddPlayerToMarketAsync(string owner, int playerId, long price);
    }
}