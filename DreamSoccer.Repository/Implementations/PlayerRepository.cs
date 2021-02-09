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
            return query.Include(n => n.Team)
                .ThenInclude(n=>n.Players)
                .Where(n => n.TeamId == teamId);
        }
        public override async Task<Player> GetByIdAsync(int id)
        {
            return (await GetAllAsync())
                .Include(n=>n.Team)
                .ThenInclude(n => n.Players)
                .Include(n => n.Team)
                .ThenInclude(n=>n.Owner)
                .FirstOrDefault(n => n.Id == id);
        }
    }
}