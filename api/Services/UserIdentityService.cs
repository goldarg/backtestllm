using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        public string[] ListarRolesDelUsuario(ClaimsPrincipal user)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();
            
            var roles = user.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
            .Select(x => x.Value).ToArray();
            
            return roles;
        }

        public string[] ListarEmpresasDelUsuario(ClaimsPrincipal user)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();
            
            var roles = user.Claims.Where(x => x.Type == "empresas")
            .Select(x => x.Value).ToArray();
            
            return roles;
        }

        public bool UsuarioPoseeRol(ClaimsPrincipal user, string rol)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;
            
            return user.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
                .Select(x => x.Value).Any(x => x == rol);
        }

        public bool UsuarioPoseeEmpresa(ClaimsPrincipal user, string empresa)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;
            
            return user.Claims.Where(x => x.Type == "empresas")
                .Select(x => x.Value).Any(x => x == empresa);
        }
    }
}