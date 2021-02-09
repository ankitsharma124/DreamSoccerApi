using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DreamSoccerApi.Helpers
{
    public class CurrentUserRepository : ICurrentUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string Email => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(n => n.Type == ClaimTypes.Name)?.Value;

        public RoleEnum Role
        {
            get
            {

                var role = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(n => n.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    return RoleEnum.Anonymous;
                }
                return (RoleEnum)Enum.Parse(typeof(RoleEnum), role);
            }
        }
    }
}
