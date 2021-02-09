using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface ITransferListService : IMessageService
    {
        Task<IEnumerable<SearchResultDto>> SearchPlayerInMarketAsync(SearchPlayerFilter input);

        Task<BuyPlayerResult> BuyPlayerAsync(int transferId, string owner, int teamId = -1);
    }
}