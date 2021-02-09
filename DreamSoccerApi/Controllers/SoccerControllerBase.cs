using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;

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
}