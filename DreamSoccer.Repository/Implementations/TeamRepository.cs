using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
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
    }
}