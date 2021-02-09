using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Extensions;
using DreamSoccer.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Repository.Implementations
{
    public class PlayerRepository : BaseRepository<int, Player>, IPlayerRepository
    {
        public PlayerRepository(DataContext context, ICurrentUserRepository currentUserRepository) : base(context, currentUserRepository)
        {
        }

        public async Task<IQueryable<Player>> GetPlayerByTeamIdAsync(int teamId)
        {
            var query = await GetAllAsync();
            return query.Include(n => n.Team)
                .ThenInclude(n => n.Players)
                .Where(n => n.TeamId == teamId);
        }
        public override async Task<Player> GetByIdAsync(int id)
        {
            return (await GetAllAsync())
                .Include(n => n.Team)
                .ThenInclude(n => n.Players)
                .Include(n => n.Team)
                .ThenInclude(n => n.Owner)
                .FirstOrDefault(n => n.Id == id);
        }

        public async Task<IQueryable<Player>> SearchAsync(SearchPlayerFilter input)
        {
            var query = await GetAllAsync();
            query = query
                    .Include(n => n.Team)
                        .ThenInclude(n => n.Players)
                    .Include(n => n.Team)
                    .ThenInclude(n => n.Owner).
                    WhereIf(!string.IsNullOrEmpty(input.Country), n => n.Country.Contains(input.Country))
                    .WhereIf(!string.IsNullOrEmpty(input.PlayerName), n => n.FirstName.Contains(input.PlayerName) || n.LastName.Contains(input.PlayerName) || (n.FirstName + ' ' + n.LastName).Contains(input.PlayerName))
                    .WhereIf(!string.IsNullOrEmpty(input.TeamName), n => n.Team.TeamName.Contains(input.TeamName))
                    .WhereIf(input.MinValue != null, n => n.Value >= input.MinValue)
                    .WhereIf(input.MaxValue != null, n => n.Value <= input.MaxValue);
            return query;
        }
    }
}