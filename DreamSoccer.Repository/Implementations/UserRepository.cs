using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;
using Microsoft.EntityFrameworkCore;
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
            return (await GetAllAsync())
                .Include(n=>n.Team)
                .ThenInclude(n => n.Players)
                .FirstOrDefault(n => n.Email == email);
        }
        public override async Task<User> GetByIdAsync(int id)
        {
            return (await GetAllAsync())
                .Include(n => n.Team)
                .ThenInclude(n => n.Players)
                .FirstOrDefault(n => n.Id == id);
        }
    }
}