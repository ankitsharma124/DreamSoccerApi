using System;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;

namespace DreamSoccer.Core.Contracts.Services
{
    public class UserService : IUserService
    {
        const long DEFAULT_BUDGET_TEAM = 5000000;
        const long DEFAULT_VALUE_PLAYER = 1000000;
        const int COUNT_GOAL_KEEPERS = 3;
        const int COUNT_DEFENDERS = 6;
        const int COUNT_MIDIFIELDERS = 6;
        const int COUNT_ATTACKERS = 5;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRandomRepository _randomRepository;

        public UserService(
            IAuthRepository authRepository,
            IMapper mapper,
            ITeamRepository teamRepository,
            IUnitOfWork unitOfWork,
            IRandomRepository randomRepository
            )
        {
            _authRepository = authRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _unitOfWork = unitOfWork;
            _randomRepository = randomRepository;
        }

        public Task<ServiceResponse<string>> LoginAsync(string email, string password)
        {
            return _authRepository.LoginAsync(email, password);
        }

        public async Task<ServiceResponse<int>> RegisterAsync(UserDto user, string password)
        {
            try
            {
                var newUser = _mapper.Map<User>(user);
                var userId = await _authRepository.RegisterAsync(newUser, password);
                var team = await _randomRepository.GetRandomTeam();
                team.Budget = DEFAULT_BUDGET_TEAM;
                await GeneratePlayers(team, COUNT_GOAL_KEEPERS, Entities.Enums.PositionEnum.Goalkeepers);
                await GeneratePlayers(team, COUNT_ATTACKERS, Entities.Enums.PositionEnum.Attackers);
                await GeneratePlayers(team, COUNT_MIDIFIELDERS, Entities.Enums.PositionEnum.Midfielders);
                await GeneratePlayers(team, COUNT_DEFENDERS, Entities.Enums.PositionEnum.Defenders);
                await _teamRepository.CreateAsync(team);
                newUser.Team = team;
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResponse<int>()
                {
                    Success = true,
                    Data = newUser.Id
                };
            }
            catch (Exception exeption)
            {
                return new ServiceResponse<int>()
                {
                    Success = false,
                    Message = exeption.Message
                };
            }

        }

        private async Task GeneratePlayers(Team team, int totalPlayer, Entities.Enums.PositionEnum position)
        {
            for (int i = 0; i < totalPlayer; i++)
            {
                var player = await _randomRepository.GetRandomPlayer();
                player.Position = position;
                player.Value = DEFAULT_VALUE_PLAYER;
                team.Players.Add(player);
            }
        }

        public Task<bool> UserExistAsync(string email)
        {
            return _authRepository.UserExistAsync(email);
        }
    }
}