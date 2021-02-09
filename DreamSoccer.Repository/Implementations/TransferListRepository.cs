using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Extensions;
using DreamSoccer.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DreamSoccer.Repository.Implementations
{
    public class TransferListRepository : BaseRepository<int, TransferList>, ITransferListRepository
    {
        public TransferListRepository(DataContext context, ICurrentUserRepository currentUserRepository) : base(context, currentUserRepository)
        {
        }

        public async Task<TransferList> CheckPlayerExistAsync(int playerId)
        {
            return (await GetAllAsync()).FirstOrDefault(n => n.PlayerId == playerId && n.DelFlag == false);
        }

        public override async Task<TransferList> GetByIdAsync(int id)
        {
            return (await GetAllAsync())
                .Include(n => n.Player).FirstOrDefault(n => n.Id == id);
        }
        public async Task<IQueryable<TransferList>> SearchPlayerAsync(SearchPlayerFilter input)
        {
            var query = await GetAllAsync();
            query = query
                .Include(n => n.Player).ThenInclude(n => n.Team)
                .Where(n => !n.DelFlag && !n.Player.DelFlag)
                .WhereIf(!string.IsNullOrEmpty(input.Country), n => n.Player.Country.Contains(input.Country))
                .WhereIf(!string.IsNullOrEmpty(input.PlayerName), n => n.Player.FirstName.Contains(input.PlayerName) || n.Player.LastName.Contains(input.PlayerName) || (n.Player.FirstName + ' ' + n.Player.LastName).Contains(input.PlayerName))
                .WhereIf(!string.IsNullOrEmpty(input.TeamName), n => n.Player.Team.TeamName.Contains(input.TeamName))
                .WhereIf(input.MinValue != null, n => n.Value >= input.MinValue)
                .WhereIf(input.MaxValue != null, n => n.Value <= input.MaxValue);
            return query;
        }
    }
}