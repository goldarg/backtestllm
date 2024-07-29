using System.Security.Claims;

namespace api.Services
{
    public class HttpContextClaimsProvider : IClaimsProvider
    {
        public HttpContextClaimsProvider(IHttpContextAccessor httpContextAccessor)
        {
            ClaimsPrincipal = httpContextAccessor?.HttpContext?.User;
        }

        public ClaimsPrincipal ClaimsPrincipal { get; }
    }

    public interface IClaimsProvider
    {
        ClaimsPrincipal ClaimsPrincipal { get; }
    }
}