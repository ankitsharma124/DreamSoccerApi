using DreamSoccer.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IPlayerRepository : IBaseRepository<int, Player>
    {
        Task<IQueryable<Player>> GetPlayerByTeamIdAsync(int teamId);
    }
}