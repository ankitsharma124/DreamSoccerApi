using System.Threading.Tasks;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext context;

        public UnitOfWork(DataContext context)
        {
            this.context = context;
        }
        public Task SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}