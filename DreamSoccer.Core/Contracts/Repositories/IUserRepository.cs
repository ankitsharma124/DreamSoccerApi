using DreamSoccer.Core.Entities;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IUserRepository : IBaseRepository<int, User>
    {
        Task<User> GetByEmailAsync(string email);
    }
}