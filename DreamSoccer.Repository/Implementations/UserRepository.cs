using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Repository.Implementations
{
    public class UserRepository : BaseRepository<int, User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return (await GetAllAsync()).FirstOrDefault(n => n.Email == email);
        }
    }
}