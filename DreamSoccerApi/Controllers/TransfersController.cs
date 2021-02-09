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
using DreamSoccer.Core.Dtos.TransferList;
using AutoMapper;
using DreamSoccer.Core.Requests;

namespace DreamSoccerApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TransfersController : SoccerControllerBase
    {
        private readonly ITransferListService _transferListService;
        private readonly IMapper _mapper;

        public TransfersController(ITransferListService transferListService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) : base(httpContextAccessor)
        {
            _transferListService = transferListService;
            _mapper = mapper;
        }

        #region SearchPlayer

        [HttpPost("SearchPlayers")]
        [Authorize(Roles = "Team_Owner,Admin")]
        public async Task<IActionResult> GetSearchPlayerAsync(SearchPlayerRequest request)
        {
            var response = new ServiceResponse<IEnumerable<SearchResultDto>>();
            try
            {
                var filter = _mapper.Map<SearchPlayerFilter>(request);
                var players = await _transferListService.SearchPlayerInMarketAsync(filter);
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

        #endregion

        #region Buy

        [HttpPost("Buy")]
        [Authorize(Roles = "Team_Owner,Admin")]
        public async Task<IActionResult> BuyPlayerAsync(BuyPlayerRequest request)
        {
            var response = new ServiceResponse<BuyPlayerResultResponse>();
            try
            {
                var email = CurrentEmail;
                var result = await _transferListService.BuyPlayerAsync(request.TransferId, email, request.TeamId);

                if (response.Success)
                {
                    if (result != null)
                    {
                        response.Data = _mapper.Map<BuyPlayerResultResponse>(result);
                        return Ok(response);
                    }
                }
                response.Success = false;
                response.Message = _transferListService.CurrentMessage;
                return NotFound(response);
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