using api.Models.DTO.Contrato;

namespace api.Services
{
    public interface IContratoService
    {
        Task<List<ContratoResponse>>? GetContratos(System.Security.Claims.ClaimsPrincipal User);
    }
}