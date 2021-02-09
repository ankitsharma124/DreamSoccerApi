using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Repositories
{
    public interface ITransferListRepository : IBaseRepository<int, TransferList>
    {
        Task<IQueryable<TransferList>> SearchPlayerAsync(SearchPlayerFilter input);
        Task<bool> CheckPlayerExistAsync(int playerId);
    }
}