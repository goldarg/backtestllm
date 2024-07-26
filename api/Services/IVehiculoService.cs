using System.Security.Claims;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Vehiculo;

namespace api.Services
{
    public interface IVehiculoService
    {
        Task<string?> AsignarVehiculo(AsignarVehiculoDto asignarVehiculoDto);
        Task<List<VehiculoDto>?> GetVehiculos(ClaimsPrincipal User);
        Task<List<OperacionesVehiculoDto>> HistorialOperaciones(ClaimsPrincipal User,
            string dominio, string tipoContrato);
    }
}