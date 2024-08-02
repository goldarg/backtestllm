using api.Models.DTO.Contrato;

namespace api.Services
{
    public interface IContratoService
    {
        Task<ContratosResponse?> GetContratos();
    }
}
