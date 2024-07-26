using System.Security.Claims;
using api.Models.DTO.Conductor;
using api.Models.DTO.User;
using api.Models.Entities;

namespace api.Services
{
    public interface IUserService
    {
        Task<List<ConductorDto>>? GetListaUsuarios(ClaimsPrincipal User);
        List<ConductorEmpresaDto> GetConductores(ClaimsPrincipal User);
        User? GetUserById(int id);
        Task DesactivarUsuario(DesactivarConductorDto desactivarDto, ClaimsPrincipal User);
        Task CreateUser(UserDto userDto, ClaimsPrincipal User);
        Task EditUser(ClaimsPrincipal User, string usuarioCrmId, UserDto userDto);
        Task EditSelfConductor(UpdateSelfConductorDto conductorDto, string userName);

        // Usuarios Empresas
        List<UsuariosEmpresas>? GetAllUsuariosEmpresas();
        List<Empresa?> GetEmpresasDelUsuario(int usuarioId);
        UsuariosEmpresas? GetUsuariosEmpresasById(int id);

        // Usuarios Roles
        List<UsuariosRoles>? GetAllUsuariosRoles();
    }
}
