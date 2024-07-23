using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.DataAccess;
using api.Models.Entities;

namespace api.Services
{
    public interface IUserIdentityService
    {
        string[] ListarRolesDelUsuario(ClaimsPrincipal user);
        string[] ListarEmpresasDelUsuario(ClaimsPrincipal user);
        bool UsuarioPoseeRol(ClaimsPrincipal user, string rol);
        bool UsuarioPoseeEmpresa(ClaimsPrincipal user, string empresa);
        Rol[] ListarRolesInferiores(ClaimsPrincipal user);
        Rol[] ListarRolesSuperiores(ClaimsPrincipal user);
        int GetJerarquiaRolMayor(ClaimsPrincipal user);
    }
}