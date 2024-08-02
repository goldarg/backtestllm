using api.Models.DTO.ActividadUsuarios;
using api.Models.DTO.Contrato;

namespace api.Services
{
    public interface IActividadUsuarioService
    {
        IList<ActividadUsuarioDto> GetActividadUsuarioEjecutor();
        IList<ActividadUsuarioDto> GetActividadUsuarioAfectado();
        void CrearActividadDb(int usuarioAfectadoId, string descripcion);
        void CrearActividadCrm(string usuarioAfectadoCrmId, string descripcion);
        void CrearRegistroActividad(
            int usuarioAfectadoId,
            int usuarioEjecutorId,
            string descripcion
        );
    }
}
