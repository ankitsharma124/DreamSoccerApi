using System.Threading.Tasks;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Responses;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<int>> RegisterAsync(UserDto user, string password);
        Task<ServiceResponse<string>> LoginAsync(string email, string password);
        Task<bool> UserExistAsync(string email);
    }
}