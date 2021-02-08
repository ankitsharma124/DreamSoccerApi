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
    [ApiController]
    [Route("[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeamController(ITeamService teamService,
            IHttpContextAccessor httpContextAccessor)
        {
            _teamService = teamService;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Signup
        [HttpGet("GetMyPlayers")]
        [Authorize(Roles = "Team_Owner")]
        public async Task<IActionResult> GetMyPlayersAsync()
        {
            var response = new ServiceResponse<IEnumerable<PlayerDto>>();
            try
            {
                var email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(n => n.Type == ClaimTypes.Name).Value;
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
        #endregion


    }
}