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
    public class TeamRepository : BaseRepository<int, Team>, ITeamRepository
    {
        public TeamRepository(DataContext context) : base(context)
        {
        }
        public override async Task<Team> GetByIdAsync(int id)
        {
            var query = await GetAllAsync();
            return query.Include(n => n.Players)
               .FirstOrDefault(n => n.Id == id);
        }

        public async Task<IQueryable<Team>> SearchTeams(SearchTeamFilter input)
        {
            return (await GetAllAsync())
                .Include(n => n.Players)
                .Include(n => n.Owner)
                .WhereIf(!string.IsNullOrEmpty(input.Country), n => n.Country.Contains(input.Country))
                .WhereIf(!string.IsNullOrEmpty(input.TeamName), n => n.TeamName.Contains(input.TeamName));
        }
    }
}