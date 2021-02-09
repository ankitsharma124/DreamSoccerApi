using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Core.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Collections.Generic;
using DreamSoccer.Core.Requests;
using AutoMapper;
using DreamSoccer.Core.Dtos.TransferList;

namespace DreamSoccerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamController : SoccerControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) : base(httpContextAccessor)
        {
            _teamService = teamService;
            _mapper = mapper;
        }
        #region GetMyPlayers

        [HttpGet("GetMyPlayers")]
        [Authorize(Roles = "Team_Owner")]
        public async Task<IActionResult> GetMyPlayersAsync()
        {
            var response = new ServiceResponse<IEnumerable<PlayerDto>>();
            try
            {
                var email = CurrentEmail;
                var players = await _teamService.GetMyTeamAsync(email);
                response.Data = players;
                if (response.Success)
                {
                    if (players.Any())
                        return Ok(response);
                }
                response.Success = false;
                return NotFound(response);
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.Success = false;
                return BadRequest(response);
            }

        }

        [HttpPost("GetAllPlayers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPlayers(SearchPlayerRequest request)
        {
            var response = new ServiceResponse<IEnumerable<PlayerDto>>();
            try
            {
                var email = CurrentEmail;
                var players = await _teamService.GetAllPlayersAsync(_mapper.Map<SearchPlayerFilter>(request));
                response.Data = players;
                if (response.Success)
                {
                    if (players.Any())
                        return Ok(response);
                }
                response.Success = false;
                return NotFound(response);
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.Success = false;
                return BadRequest(response);
            }

        }

        [HttpPost("AddPlayerToMarket")]
        [Authorize(Roles = "Team_Owner,Admin")]
        public async Task<IActionResult> AddPlayerToMarketAsycn(AddTransferListRequest request)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var email = CurrentEmail;
                var result = await _teamService.AddPlayerToMarketAsync(email, request.PlayerId, request.Price);
                response.Data = result;
                if (response.Success && result)
                {
                    return Ok(response);
                }
                else
                {
                    response.Success = false;
                    response.Message = _teamService.CurrentMessage;
                    return BadRequest(response);
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.Success = false;
                return BadRequest(response);
            }

        }
        #endregion


    }
}