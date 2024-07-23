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
        //Devuelve todos los nombreRol de los roles asignados al usuario
        public string[] ListarRolesDelUsuario(ClaimsPrincipal user)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();

            var roles = user.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
            .Select(x => x.Value).ToArray();

            return roles;
        }

        //Devuelve todos los CrmId de las empresas que posee el usuario
        public string[] ListarEmpresasDelUsuario(ClaimsPrincipal user)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();

            var roles = user.Claims.Where(x => x.Type == "empresas")
            .Select(x => x.Value).ToArray();

            return roles;
        }

        //Devuelve si el usuario tiene o no el claim del nombreRol
        public bool UsuarioPoseeRol(ClaimsPrincipal user, string rol)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;

            return user.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
                .Select(x => x.Value).Any(x => x == rol);
        }

        //Devuelve si el usuario tiene o no el claim al CrmId de la empresa
        public bool UsuarioPoseeEmpresa(ClaimsPrincipal user, string empresa)
        {
            var claimIdentity = user.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;

            return user.Claims.Where(x => x.Type == "empresas")
                .Select(x => x.Value).Any(x => x == empresa);
        }

        //Devuelve la Jerarquia del Rol mas alto del usuario
        public int GetJerarquiaRolMayor(ClaimsPrincipal user, IRdaUnitOfWork unitOfWork)
        {
            var rolesUsuario = ListarRolesDelUsuario(user);
            if (rolesUsuario == null || !rolesUsuario.Any())
                return 0;

            var jerarquiaRolMayor = unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => rolesUsuario.Contains(r.nombreRol))
                .OrderByDescending(r => r.jerarquia)
                .Select(x => x.jerarquia)
                .FirstOrDefault();

            return jerarquiaRolMayor;
        }

        //Lista los roles superiores, INCLUYENDO al mayor rol, del usario
        public Rol[] ListarRolesSuperiores(ClaimsPrincipal user, IRdaUnitOfWork unitOfWork)
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor(user, unitOfWork);
            
            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesSuperiores = unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia >= rolConMayorJerarquia)
                .ToArray();

            return rolesSuperiores;
        }

        public Rol[] ListarRolesInferiores(ClaimsPrincipal user, IRdaUnitOfWork unitOfWork)
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor(user, unitOfWork);

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