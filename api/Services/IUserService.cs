using System.Security.Claims;
using api.Models.DTO.Conductor;
using api.Models.DTO.User;
using api.Models.Entities;

namespace api.Services
{
    public interface IUserService
    {
        Task<List<ConductorDto>>? GetListaUsuarios();
        List<ConductorEmpresaDto> GetConductores();
        User? GetUserById(int id);
        Task DesactivarUsuario(DesactivarConductorDto desactivarDto);
        Task CreateUser(UserDto userDto);
        Task EditUser(string usuarioCrmId, UserDto userDto);
        Task EditSelfConductor(UpdateSelfConductorDto conductorDto, string userName);

        // Usuarios Empresas
        List<UsuariosEmpresas>? GetAllUsuariosEmpresas();
        List<Empresa?> GetEmpresasDelUsuario(int usuarioId);
        UsuariosEmpresas? GetUsuariosEmpresasById(int id);

        // Usuarios Roles
        List<UsuariosRoles>? GetAllUsuariosRoles();
    }
}
