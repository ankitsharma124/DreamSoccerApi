using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Contracts.Services
{
    public class TeamService : BaseService, ITeamService
    {
        private IMapper _mapper;
        private IUserRepository _userRepository;
        private IPlayerRepository _playerRepository;
        private readonly ITransferListRepository _transferListRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(IMapper mapper,
            IUserRepository userRepository,
            IPlayerRepository playerRepository,
            ITransferListRepository transferListRepository,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _transferListRepository = transferListRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlayerDto>> GetMyTeamAsync(string email)
        {
            var user = (await _userRepository.GetAllAsync()).FirstOrDefault(n => n.Email == email);
            if (user != null)
            {
                var players = await _playerRepository.GetPlayerByTeamIdAsync(user.TeamId.Value);
                return _mapper.Map<List<PlayerDto>>(players);
            }
            return new List<PlayerDto>();

        }

        public async Task<bool> AddPlayerToMarketAsync(string owner, int playerId, long price)
        {
            var user = await _userRepository.GetByEmailAsync(owner);
            if (user == null)
            {
                CurrentMessage = "User doesn't exist";
                return false;
            }
            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player != null)
                if (player.Team.Owner.Email == user.Email)
                {
                    if (await _transferListRepository.CheckPlayerExistAsync(player.Id))
                    {
                        CurrentMessage = "Player Exist in transfer List";
                        return false;
                    }
                    var model = new TransferList() { PlayerId = player.Id, Value = price };
                    await _transferListRepository.CreateAsync(model);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            CurrentMessage = "Player not in our Team";
            return false;
        }
    }
}