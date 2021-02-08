using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Repository.Implementations
{
    public class PlayerRepository : BaseRepository<int, Player>, IPlayerRepository
    {
        public PlayerRepository(DataContext context) : base(context)
        {
        }

        public async Task<IQueryable<Player>> GetPlayerByTeamIdAsync(int teamId)
        {
            var query = await GetAllAsync();
            return query.Include(n => n.Team).Where(n => n.TeamId == teamId);
        }
    }
}