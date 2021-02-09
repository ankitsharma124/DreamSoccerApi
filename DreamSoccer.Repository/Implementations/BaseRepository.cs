using System;
using System.Linq;
using System.Threading.Tasks;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class BaseRepository<TKey, TEntity> : IBaseRepository<TKey, TEntity>
        where TEntity : BaseEntity
    {
        private readonly DataContext _context;
        private readonly ICurrentUserRepository _currentUserRepository;

        public BaseRepository(DataContext context,
            ICurrentUserRepository currentUserRepository)
        {
            _context = context;
            _currentUserRepository = currentUserRepository;
        }
        public virtual async Task<TEntity> CreateAsync(TEntity model)
        {
            model.CreatedAt = DateTime.Now;
            await _context.Set<TEntity>().AddAsync(model);
            return model;

        }

        public virtual async Task DeleteAsync(TKey id)
        {
            var model = await GetByIdAsync(id);
            model.DelFlag = true;
            
            _context.Set<TEntity>().Update(model);
        }

        public virtual Task<IQueryable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(_context.Set<TEntity>().Where(n => !n.DelFlag));
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return (await _context.FindAsync<TEntity>(id));
        }

        public virtual async Task<TEntity> UpdateAsync(TKey id, TEntity model)
        {
            model.UpdateAt = DateTime.Now;
            model.UpdatedBy = _currentUserRepository.Email;
            _context.Set<TEntity>().Update(model);
            model = await GetByIdAsync(id);
            return model;
        }
    }
}