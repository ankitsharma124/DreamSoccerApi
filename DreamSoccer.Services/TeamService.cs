using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;

namespace DreamSoccer.Core.Contracts.Services
{
    public class TeamService : ITeamService
    {
        private IMapper _mapper;
        private IUserRepository _userRepository;
        private IPlayerRepository _playerRepository;

        public TeamService(IMapper mapper, IUserRepository userRepository, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
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
    }
}