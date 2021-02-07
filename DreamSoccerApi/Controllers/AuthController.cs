using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DreamSoccerApi.Data;
using DreamSoccerApi.Dtos.User;
using DreamSoccerApi.Models;
using DreamSoccerApi;

namespace Toptal.Controllers
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
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto _newUser)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (!ModelState.IsValid)
            {
                return BadRequest(response);;
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
        
    }
}