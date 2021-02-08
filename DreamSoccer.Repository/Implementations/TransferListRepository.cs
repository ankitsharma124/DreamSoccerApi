using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using DreamSoccer.Repository.Context;

namespace DreamSoccer.Repository.Implementations
{
    public class TransferListRepository : BaseRepository<int, TransferList>, ITransferListRepository
    {
        public TransferListRepository(DataContext context) : base(context)
        {
        }
    }
}