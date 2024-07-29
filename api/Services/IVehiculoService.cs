using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Vehiculo;

namespace api.Services
{
    public interface IVehiculoService
    {
        Task<string?> AsignarVehiculo(AsignarVehiculoDto asignarVehiculoDto);
        Task<List<VehiculoDto>?> GetVehiculos();
        Task<List<OperacionesVehiculoDto>> HistorialOperaciones(string dominio, string tipoContrato);
    }
}