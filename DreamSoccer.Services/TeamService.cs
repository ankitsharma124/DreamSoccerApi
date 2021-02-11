﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.Players;
using DreamSoccer.Core.Dtos.Teams;
using DreamSoccer.Core.Dtos.TransferList;
using DreamSoccer.Core.Dtos.User;
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
        private readonly ICurrentUserRepository _currentUserRepository;

        public TeamService(IMapper mapper,
            IUserRepository userRepository,
            IPlayerRepository playerRepository,
            ITransferListRepository transferListRepository,
            IUnitOfWork unitOfWork,
            ITeamRepository teamRepository,
            ICurrentUserRepository currentUserRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _transferListRepository = transferListRepository;
            _unitOfWork = unitOfWork;
            _teamRepository = teamRepository;
            _currentUserRepository = currentUserRepository;
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

        public async Task<int> AddPlayerToMarketAsync(string owner, int playerId, long price)
        {
            var user = await _userRepository.GetByEmailAsync(owner);
            if (user == null)
            {
                CurrentMessage = "User doesn't exist";
                return -1;
            }
            var player = await _playerRepository.GetByIdAsync(playerId);

            if (player != null)
            {
                var currentPlayerInTransferList = await CheckInTransferList(player);
                if (player.Team == null && currentPlayerInTransferList != null)
                {
                    CurrentMessage = "Player Exist in transfer List";
                    return -1;
                }
                if (player.Team.Owner.Email == user.Email || _currentUserRepository.Role == RoleEnum.Admin)
                {
                    var model = new TransferList() { PlayerId = player.Id, Value = price };
                    player.PreviousTeam = player.TeamId.Value;
                    player.TeamId = null;
                    await _playerRepository.UpdateAsync(player.Id, player);
                    model = await _transferListRepository.CreateAsync(model);
                    await _unitOfWork.SaveChangesAsync();
                    return model.Id;
                }
            }
            CurrentMessage = "Player not in our Team";
            return -1;
        }

        private async Task<TransferList> CheckInTransferList(Player player)
        {
            var transferListPlayer = await _transferListRepository.CheckPlayerExistAsync(player.Id);
            if (transferListPlayer != null)
            {
                return transferListPlayer;
            }
            return null;
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

        public async Task<TeamDto> UpdateTeamAsync(TeamDto team)
        {
            var currentTeam = await _teamRepository.GetByIdAsync(team.Id);
            if (_currentUserRepository.Role == RoleEnum.Team_Owner)
            {
                var user = await _userRepository.GetByEmailAsync(_currentUserRepository.Email);
                if (team.Id != user.TeamId)
                {
                    CurrentMessage = "It's not your team";
                    return null;
                }
                if (user.Team.Budget != team.Budget)
                {
                    CurrentMessage = "You can't change budget";
                    return null;
                }
            }
            var input = _mapper.Map<TeamInput>(team);
            input.Owner = _mapper.Map<UserOwnerDto>(currentTeam.Owner);
            input.Players = _mapper.Map<List<PlayerDto>>(currentTeam.Players);

            currentTeam = _mapper.Map<TeamInput, Team>(input, currentTeam);
            await _teamRepository.UpdateAsync(team.Id, currentTeam);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TeamDto>(currentTeam);
        }

        public async Task<PlayerDto> UpdatePlayerAsync(PlayerDto player)
        {
            var currentPlayer = await _playerRepository.GetByIdAsync(player.Id);
            if (_currentUserRepository.Role == RoleEnum.Team_Owner)
            {
                var user = await _userRepository.GetByEmailAsync(_currentUserRepository.Email);
                if (currentPlayer.TeamId != user.TeamId)
                {
                    CurrentMessage = "Player not in your team";
                    return null;
                }

                if (currentPlayer.Value != player.Value)
                {
                    CurrentMessage = "You can't change value";
                    return null;
                }
            }
            player.PreviousTeam = currentPlayer.PreviousTeam;
            player.TeamId = currentPlayer.TeamId;
            player.Team = _mapper.Map<TeamDto>(currentPlayer.Team);
            player = _mapper.Map<PlayerDto>(await _playerRepository.UpdateAsync(player.Id, _mapper.Map<PlayerDto, Player>(player, currentPlayer)));
            await _unitOfWork.SaveChangesAsync();
            return player;
        }

        public async Task<PlayerDto> DeletePlayerAsync(PlayerDto player)
        {
            if (_currentUserRepository.Role == RoleEnum.Team_Owner)
            {
                var user = await _userRepository.GetByEmailAsync(_currentUserRepository.Email);
                if (player.TeamId != user.TeamId)
                {
                    CurrentMessage = "Player not in your team";
                    return null;
                }
            }
            await _playerRepository.DeleteAsync(player.Id);
            await _unitOfWork.SaveChangesAsync();
            return player;
        }
    }
}