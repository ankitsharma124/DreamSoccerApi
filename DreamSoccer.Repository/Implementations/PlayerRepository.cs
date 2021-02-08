using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class PlayerRepository : BaseRepository<int, Player>, IPlayerRepository
    {
        public PlayerRepository(DataContext context) : base(context)
        {
        }
    }
}