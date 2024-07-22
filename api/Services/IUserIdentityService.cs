using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IUserIdentityService
    {
        string[] ListarRolesDelUsuario(ClaimsPrincipal user);
        string[] ListarEmpresasDelUsuario(ClaimsPrincipal user);
        bool UsuarioPoseeRol(ClaimsPrincipal user, string rol);
        bool UsuarioPoseeEmpresa(ClaimsPrincipal user, string empresa);
    }
}