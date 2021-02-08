using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Core.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace DreamSoccerApi.Controllers
{
    public class SoccerControllerBase : ControllerBase
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public SoccerControllerBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string CurrentEmail
        {
            get
            {
                return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.Name).Value;
            }
        }
    }
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

        #region Signup

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
                var result = await _teamService.AddPlayerToMarket(email, request.PlayerId, request.Price);
                response.Data = result;
                if (response.Success && result)
                {
                    return Ok(response);
                }
                else
                {
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