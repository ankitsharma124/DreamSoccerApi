using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace DreamSoccerApi_Test.Helpers
{
    public static class AuthorizationHelper
    {
        public static Mock<IHttpContextAccessor> CreateUserLogin(this Mock<IHttpContextAccessor> httpContextAccessor, int userId)
        {

            var identity = new GenericIdentity(userId.ToString());
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed
            var context = new DefaultHttpContext()
            {
                User = contextUser
            };
            httpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return httpContextAccessor;

        }
    }
}