using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class UserRepository : BaseRepository<int, User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }
    }
}