using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DreamSoccer.Core.Contracts.Services
{
    public interface ITransferListService
    {
        Task<IEnumerable<PlayerDto>> SearchPlayerInMarketAsync(SearchPlayerFilter input);

    }
}