using System.Linq;
using System.Threading.Tasks;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface IBaseRepository<TKey, TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity> CreateAsync(TEntity model);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<TEntity> UpdateAsync(TKey id, TEntity model);
        Task DeleteAsync(TKey id);
        Task<IQueryable<TEntity>> GetAllAsync();
    }
}