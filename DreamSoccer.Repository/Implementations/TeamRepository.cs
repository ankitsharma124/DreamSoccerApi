using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class TeamRepository : BaseRepository<int, Team>, ITeamRepository
    {
        public TeamRepository(DataContext context) : base(context)
        {
        }
    }
}