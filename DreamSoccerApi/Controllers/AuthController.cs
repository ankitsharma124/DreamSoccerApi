using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using DreamSoccer.Core.Responses;
using DreamSoccer.Core.Contracts.Services;

namespace DreamSoccerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }
        #region Signup
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto _newUser)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(response);
            }
            response = await _userService.RegisterAsync(
                new UserDto { Email = _newUser.Email, Role = _newUser.Role }, _newUser.Password
            );
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto _newUser)
        {
            ServiceResponse<string> response = await _userService.LoginAsync(
                _newUser.Email, _newUser.Password
            );
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        #endregion
    }
}