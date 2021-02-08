using System.Threading.Tasks;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string email, string password);
        Task<bool> UserExist(string email);
    }
}