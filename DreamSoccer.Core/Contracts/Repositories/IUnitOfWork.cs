using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}