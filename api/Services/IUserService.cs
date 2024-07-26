using api.Models.DTO.Conductor;
using api.Models.DTO.User;
using api.Models.Entities;

namespace api.Services
{
    public interface IUserService
    {
        Task<List<ConductorDto>>? GetListaUsuarios(System.Security.Claims.ClaimsPrincipal User);
        List<ConductorEmpresaDto> GetConductores(System.Security.Claims.ClaimsPrincipal User);
        User? GetUserById(int id);
        Task DesactivarUsuario(string usuarioCrmId, System.Security.Claims.ClaimsPrincipal User);
        Task CreateUser(CreateUserDto userDto, System.Security.Claims.ClaimsPrincipal User);

        // Usuarios Empresas
        List<UsuariosEmpresas>? GetAllUsuariosEmpresas();
        List<Empresa?> GetEmpresasDelUsuario(int usuarioId);
        UsuariosEmpresas? GetUsuariosEmpresasById(int id);

        // Usuarios Roles
        List<UsuariosRoles>? GetAllUsuariosRoles();
    }
}