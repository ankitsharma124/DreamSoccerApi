using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamSoccerApi.Data;
using DreamSoccerApi.Models;
using DreamSoccerApi;
using DreamSoccerApi.Dtos.User;

namespace DreamSoccerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;   
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
            response = await _authRepo.Register(
                new User { Email = _newUser.Email, Role = _newUser.Role}, _newUser.Password
            );
            if(response.Success)
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
            ServiceResponse<string> response = await _authRepo.Login(
                _newUser.Email, _newUser.Password
            );
            if(response.Success)
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