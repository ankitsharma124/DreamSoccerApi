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

        public BaseRepository(DataContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TEntity model)
        {
            await _context.Set<TEntity>().AddAsync(model);

        }

        public async Task DeleteAsync(TKey id)
        {
            var model = await GetByIdAsync(id);
            model.DelFlag = true;
            _context.Set<TEntity>().Update(model);
        }

        public Task<IQueryable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(_context.Set<TEntity>().Where(n => !n.DelFlag));
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return (await _context.FindAsync<TEntity>(id));
        }

        public Task UpdateAsync(TKey id, TEntity model)
        {
            model.UpdateAt = DateTime.Now;
            return Task.FromResult(_context.Set<TEntity>().Update(model));
        }
    }
}