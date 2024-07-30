using api.Models.Entities;

namespace api.Services
{
    public interface IUserIdentityService
    {
        string[] ListarRolesDelUsuario();

        /// <summary>
        /// Devuelve los idCRM de las empresas a las que el usuario tiene acceso.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>idCRM</returns>
        string[] ListarEmpresasDelUsuario();
        bool UsuarioPoseeRol(string rol);
        bool UsuarioPoseeEmpresa(string empresa);
        Rol[] ListarRolesInferiores();
        Rol[] ListarRolesSuperiores();
        int GetJerarquiaRolMayor();

        bool EsInferiorEnRoles(User user);
    }
}
