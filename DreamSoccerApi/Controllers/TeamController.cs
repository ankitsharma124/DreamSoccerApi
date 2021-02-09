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


namespace DreamSoccerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamController : SoccerControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _teamService = teamService;
        }
        [HttpGet("TestIndex")]
        [Authorize(Roles = "Team_Owner")]
        public IActionResult GetIndex()
        {
            return Ok();
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
                if (response.Success && players.Any())
                {
                    return Ok(response);
                }
                else
                {
                    response.Success = false;
                    return NotFound(response);
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
                response.Success = false;
                return BadRequest(response);
            }

        }

        [HttpPost("AddPlayerToMarket")]
        [Authorize(Roles = "Team_Owner")]
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