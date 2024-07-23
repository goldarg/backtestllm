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
        private readonly IRdaUnitOfWork _unitOfWork;
        public UserIdentityService(IRdaUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        /// <summary>
        /// Devuelve la jerarquía del rol más alto del usuario.
        /// </summary>
        /// <param name="user">El usuario cuyas jerarquías de roles se evaluarán.</param>
        /// <returns>La jerarquía del rol más alto del usuario. Retorna 0 si el usuario no tiene roles.</returns>
        public int GetJerarquiaRolMayor(ClaimsPrincipal user)
        {
            var rolesUsuario = ListarRolesDelUsuario(user);
            if (rolesUsuario == null || !rolesUsuario.Any())
                return 0;

            var jerarquiaRolMayor = _unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => rolesUsuario.Contains(r.nombreRol))
                .OrderByDescending(r => r.jerarquia)
                .Select(x => x.jerarquia)
                .FirstOrDefault();

            return jerarquiaRolMayor;
        }

        //Lista los roles superiores, INCLUYENDO al mayor rol, del usario
        public Rol[] ListarRolesSuperiores(ClaimsPrincipal user)
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor(user);

            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesSuperiores = _unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia >= rolConMayorJerarquia)
                .ToArray();

            return rolesSuperiores;
        }

        public Rol[] ListarRolesInferiores(ClaimsPrincipal user)
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor(user);

            if (rolConMayorJerarquia == 0)
                return Array.Empty<Rol>();

            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesInferiores = _unitOfWork.GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia < rolConMayorJerarquia)
                .ToArray();

            return rolesInferiores;
        }


    }
}