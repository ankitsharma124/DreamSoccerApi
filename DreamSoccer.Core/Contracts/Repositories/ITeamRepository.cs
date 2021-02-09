using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface ITeamRepository : IBaseRepository<int, Team>
    {
        Task<IQueryable<Team>> SearchTeams(SearchTeamFilter input);
    }
}