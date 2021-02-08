using System.Threading.Tasks;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> RegisterAsync(User user, string password);
        Task<ServiceResponse<string>> LoginAsync(string email, string password);
        Task<bool> UserExistAsync(string email);
    }
}