using DreamSoccer.Core.Entities;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IRandomRepository
    {
        public Task<Player> GetRandomPlayer();
        public Task<Team> GetRandomTeam();
    }
}