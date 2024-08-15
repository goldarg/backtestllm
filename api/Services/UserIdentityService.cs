using System.Security.Claims;
using api.DataAccess;
using api.Exceptions;
using api.Models.Entities;

namespace api.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IRdaUnitOfWork _unitOfWork;
        private readonly IClaimsProvider _claimsProvider;

        public UserIdentityService(IClaimsProvider claimsProvider, IRdaUnitOfWork unitOfWork)
        {
            _claimsProvider = claimsProvider;
            _unitOfWork = unitOfWork;
        }

        public User GetUsuarioDb()
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                throw new BadRequestException("Ocurrió un error al validar al usuario");

            var user = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x => x.userName == claimIdentity.Name)
                .FirstOrDefault();

            if (user == null)
                throw new BadRequestException("Ocurrió un error al validar al usuario");

            return user;
        }

        //Devuelve todos los nombreRol de los roles asignados al usuario
        public string[] ListarRolesDelUsuario()
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();

            var roles = _claimsProvider
                .ClaimsPrincipal.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
                .Select(x => x.Value)
                .ToArray();

            return roles;
        }

        //Devuelve todos los CrmId de las empresas que posee el usuario
        public string[] ListarEmpresasDelUsuario()
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                return Array.Empty<string>();

            var empresas = _claimsProvider
                .ClaimsPrincipal.Claims.Where(x => x.Type == "empresas")
                .Select(x => x.Value)
                .ToArray();

            return empresas;
        }

        //Devuelve si el usuario tiene o no el claim del nombreRol
        public bool UsuarioPoseeRol(string rol)
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;

            return _claimsProvider
                .ClaimsPrincipal.Claims.Where(x => x.Type == claimIdentity.RoleClaimType)
                .Select(x => x.Value)
                .Any(x => x == rol);
        }

        public string? UserGetCrmId()
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;

            if (claimIdentity == null)
                throw new BadRequestException("Error al obtener la identidad del usuario");

            var crmId = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x => x.userName == claimIdentity.Name)
                .Select(x => x.idCRM)
                .FirstOrDefault();

            return crmId;
        }

        //Devuelve si el usuario tiene o no el claim al CrmId de la empresa
        public bool UsuarioPoseeEmpresa(string empresa)
        {
            var claimIdentity = _claimsProvider.ClaimsPrincipal.Identity as ClaimsIdentity;
            if (claimIdentity == null)
                return false;

            return _claimsProvider
                .ClaimsPrincipal.Claims.Where(x => x.Type == "empresas")
                .Select(x => x.Value)
                .Any(x => x == empresa);
        }

        /// <summary>
        /// Devuelve la jerarquía del rol más alto del usuario.
        /// </summary>
        /// <param name="user">El usuario cuyas jerarquías de roles se evaluarán.</param>
        /// <returns>La jerarquía del rol más alto del usuario. Retorna 0 si el usuario no tiene roles.</returns>
        public int GetJerarquiaRolMayor()
        {
            var rolesUsuario = ListarRolesDelUsuario();
            if (rolesUsuario == null || !rolesUsuario.Any())
                return 0;

            var jerarquiaRolMayor = _unitOfWork
                .GetRepository<Rol>()
                .GetAll()
                .Where(r => rolesUsuario.Contains(r.nombreRol))
                .OrderByDescending(r => r.jerarquia)
                .Select(x => x.jerarquia)
                .FirstOrDefault();

            return jerarquiaRolMayor;
        }

        //Lista los roles superiores, INCLUYENDO al mayor rol, del usario
        public Rol[] ListarRolesSuperiores()
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor();

            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesSuperiores = _unitOfWork
                .GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia >= rolConMayorJerarquia)
                .ToArray();

            return rolesSuperiores;
        }

        public Rol[] ListarRolesInferiores()
        {
            var rolConMayorJerarquia = GetJerarquiaRolMayor();

            if (rolConMayorJerarquia == 0)
                return Array.Empty<Rol>();

            // Obtener todos los roles inferiores a rolConMayorJerarquia.jerarquia
            var rolesInferiores = _unitOfWork
                .GetRepository<Rol>()
                .GetAll()
                .Where(r => r.jerarquia < rolConMayorJerarquia)
                .ToArray();

            return rolesInferiores;
        }

        public bool EsInferiorEnRoles(User user)
        {
            var rolesInferiores = ListarRolesInferiores();
            // si para cada rol del usuario,hay un rolInferior que sea mayor o igual a ese rol, entonces el usuario es inferior
            bool esInferior = user.Roles.All(ur =>
                rolesInferiores.Any(ri => ur.Rol?.jerarquia <= ri.jerarquia)
            );
            return esInferior;
        }
    }
}
