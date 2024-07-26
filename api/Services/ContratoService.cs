using System.Security.Claims;
using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.Models.DTO.Contrato;

namespace api.Services
{
    public class ContratoService : IContratoService
    {
        private readonly IUserIdentityService _identityService;
        private readonly CRMService _crmService;
        
        public ContratoService(IUserIdentityService identityService, CRMService crmService)
        {
            _identityService = identityService;
            _crmService = crmService;
        }

        public async Task<List<ContratoResponse>>? GetContratos(ClaimsPrincipal User)
        {
            var empresasDisponibles = _identityService.ListarEmpresasDelUsuario(User);

            var uri = new StringBuilder("crm/v2/Contratos?fields=id,Name,Cuenta");
            var json = await _crmService.Get(uri.ToString());
            var contratos = JsonSerializer.Deserialize<List<ContratoResponse>>(json);

            //Se filtra desde el BE porque el CRM soporta un maximo de 20 operadores logicos
            return contratos.Where(x => empresasDisponibles.Contains(x.Cuenta.id)).ToList();
        }
    }
}