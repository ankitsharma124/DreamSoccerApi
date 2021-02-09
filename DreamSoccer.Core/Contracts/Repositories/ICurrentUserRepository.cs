using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface ICurrentUserRepository
    {
        string Email { get; }
        RoleEnum Role { get; }
    }
}