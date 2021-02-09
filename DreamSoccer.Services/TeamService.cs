﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Dtos.Teams;
using DreamSoccer.Core.Dtos.TransferList;
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
        private readonly ITeamRepository _teamRepository;

        public TeamService(IMapper mapper,
            IUserRepository userRepository,
            IPlayerRepository playerRepository,
            ITransferListRepository transferListRepository,
            IUnitOfWork unitOfWork,
            ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _transferListRepository = transferListRepository;
            _unitOfWork = unitOfWork;
            _teamRepository = teamRepository;
        }

        public async Task<TeamInformationDto> GetMyTeamAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null)
            {
                return _mapper.Map<TeamInformationDto>(user.Team);
            }
            return null;

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



        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync(SearchPlayerFilter input)
        {
            var players = await _playerRepository.SearchAsync(input);
            return _mapper.Map<List<PlayerDto>>(players);
        }

        public async Task<IEnumerable<TeamInformationDto>> GetAllTeams(SearchTeamFilter input)
        {
            var teams = await _teamRepository.SearchTeams(input);
            return _mapper.Map<IEnumerable<TeamInformationDto>>(teams);
        }
    }
}