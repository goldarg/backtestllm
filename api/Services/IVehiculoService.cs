using api.Models.DTO;
using api.Models.DTO.Vehiculo;

namespace api.Services
{
    public interface IVehiculoService
    {
        Task<string?> AsignarVehiculo(AsignarVehiculoDto asignarVehiculoDto);
        Task<List<VehiculoDto>?> GetVehiculos(System.Security.Claims.ClaimsPrincipal User);
    }
}