using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DataAccess;
using api.Models.Entities;

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

        public Rol[] ListarRolesInferiores(ClaimsPrincipal user, IRdaUnitOfWork unitOfWork)
        {
            var rolesUsuario = ListarRolesDelUsuario(user);
            if (rolesUsuario == null || !rolesUsuario.Any())
                return Array.Empty<Rol>();

            // Obtener el rol con mayor jerarquía que tiene el usuario
            var rolConMayorJerarquia = unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => rolesUsuario.Contains(r.nombreRol))
                .OrderByDescending(r => r.jerarquia)
                .Select(r => r.jerarquia)
                .FirstOrDefault();

            if (rolConMayorJerarquia == 0)
                return Array.Empty<Rol>();

            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesInferiores = unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia < rolConMayorJerarquia)
                .ToArray();

            return rolesInferiores;
        }


    }
}