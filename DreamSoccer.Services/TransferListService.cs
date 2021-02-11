using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Entities;
namespace DreamSoccer.Core.Contracts.Services
{
    public class TransferListService : BaseService, ITransferListService
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITransferListRepository _transferListRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRandomRepository _randomRepository;
        private readonly ICurrentUserRepository _currentUserRepository;

        public TransferListService(IMapper mapper,
            IPlayerRepository playerRepository,
            ITransferListRepository transferListRepository,
            IUnitOfWork unitOfWork,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IRandomRepository randomRepository,
            ICurrentUserRepository currentUserRepository)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
            _transferListRepository = transferListRepository;
            _unitOfWork = unitOfWork;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _randomRepository = randomRepository;
            _currentUserRepository = currentUserRepository;
        }

        public async Task<BuyPlayerResult> BuyPlayerAsync(int transferId, string owner, int teamId = -1)
        {
            var role = _currentUserRepository.Role;
            var player = await _transferListRepository.GetByIdAsync(transferId);
            if (player == null)
            {
                CurrentMessage = "Player not exists";
                return null;
            }
            var user = await _userRepository.GetByEmailAsync(owner);
            if (role != RoleEnum.Admin)
            {
                if (user?.TeamId != null && user.TeamId.HasValue)
                    teamId = user.TeamId.Value;
            }
            if (user == null)
            {
                CurrentMessage = "User not exists";
                return null;
            }
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                CurrentMessage = "Team not exists";
                return null;
            }
            if (team.Budget >= player.Value)
            {
                if (team.Id == player.Player.TeamId)
                {
                    CurrentMessage = "This player in your team";
                    return null;
                }
                var currentPlayer = player.Player;
                var previosTeam = await _teamRepository.GetByIdAsync(currentPlayer.PreviousTeam);
                var valueIncrease = await _randomRepository.GetRandomRatioForIncreaseValue();
                currentPlayer.Value = currentPlayer.Value + (valueIncrease * currentPlayer.Value / 100);
                previosTeam.Budget = previosTeam.Budget + player.Value;
                team.Budget = team.Budget - player.Value;
                currentPlayer.TeamId = team.Id;
                await _playerRepository.UpdateAsync(currentPlayer.Id, currentPlayer);
                await _teamRepository.UpdateAsync(teamId, team);
                await _teamRepository.UpdateAsync(previosTeam.Id, previosTeam);
                await _transferListRepository.DeleteAsync(player.Id);
                await _unitOfWork.SaveChangesAsync();
                currentPlayer = await _playerRepository.GetByIdAsync(currentPlayer.Id);
                return new BuyPlayerResult()
                {
                    Player = currentPlayer,
                    PreviousTeam = previosTeam,
                    NextTeam = team,

                };
            }
            else
            {
                CurrentMessage = "Budget not enough";
                return null;
            }
        }

        public async Task<IEnumerable<SearchResultDto>> SearchPlayerInMarketAsync(SearchPlayerFilter input)
        {
            IQueryable<TransferList> query = await _transferListRepository.SearchPlayerAsync(input);

            var list = new List<Player>();
            return _mapper.Map<List<SearchResultDto>>(query.Select(n => n));
        }


    }
}