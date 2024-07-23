using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.Models.DTO;
using api.Models.DTO.Operaciones;
using api.Models.DTO.Vehiculo;
using Newtonsoft.Json.Linq;

namespace api.Logic
{
    public class VehiculosLogica
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CRMService _crmService;

        public VehiculosLogica(IHttpClientFactory httpClientFactory, CRMService cRMService)
        {
            _httpClientFactory = httpClientFactory;
            _crmService = cRMService;
        }

        //Para el CRM, conductor se define como:
        //1) Buscar Renting activo con el vehiculo. Si hay, es ese conductor.
        //2) Buscar Servicio activo con el vehiculo. Si hay, es ese conductor.
        //3) Buscar Alquileres activo con el vehiculo. Si hay, es ese conductor.
        //4) Si no hay nada, entonces no tiene conductor asignado y se mostrará eso.
        //RDA valida que el vehiculo esté asignado a un único conductor/renting/etc entidad.
        public async Task<CRMRelatedObject> BuscarConductor(VehiculoDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient("CrmHttpClient");

            //Renting
            var uri = $"crm/v2/Renting/search?criteria=(Dominio.ID:equals:{dto.id})and(Estado:equals:Activo)&fields=Conductor";

            var response = await httpClient.GetAsync(uri.ToString());
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var responseDto = JsonSerializer.Deserialize<BuscarConductorResponse>(json, options);

            if (responseDto != null)
                return responseDto.Data.First().Conductor;
            
            //Modulo 2
            uri = $"Alguna URI";
            response = await httpClient.GetAsync(uri.ToString());
            json = await response.Content.ReadAsStringAsync();

            responseDto = JsonSerializer.Deserialize<BuscarConductorResponse>(json, options);

            if (responseDto != null)
                return responseDto.Data.First().Conductor;

            //Modulo 2
            uri = $"Alguna URI";
            response = await httpClient.GetAsync(uri.ToString());
            json = await response.Content.ReadAsStringAsync();

            responseDto = JsonSerializer.Deserialize<BuscarConductorResponse>(json, options);

            if (responseDto != null)
                return responseDto.Data.First().Conductor;

            //Si no, entonces no estaba asignado a nadie
            return new CRMRelatedObject();
        }
    
        public async Task<List<OperacionesVehiculoDto>> HistorialOperaciones(string[] empresasDisponibles, 
            string dominio, string tipoContrato)
        {
            var uris = new Dictionary<string, string>
            {
                { "Alquiler Corporativo", $"crm/v2/Alquileres/search?criteria=(Dominio_Alquiler.name:equals:" + dominio + ")&fields=Dominio_Alquiler,Contrato" },
                { "Fleet Management", $"crm/v2/Servicios_RDA/search?criteria=(Dominio.name:equals:" + dominio + ")&fields=Dominio,Contrato" },
                { "Renting", $"crm/v2/Renting/search?criteria=(Dominio.name:equals:" + dominio + ")&fields=Dominio,Nombre_del_contrato" }
            };

            var json = await _crmService.Get(uris[tipoContrato]);
            var contratoId = JArray.Parse(json)[0][tipoContrato == "Renting" ? "Nombre_del_contrato" : "Contrato"].ToObject<CRMRelatedObject>().id;

            var uri = new StringBuilder("crm/v2/Contratos/search?criteria=(Id:equals:" + contratoId + ")&fields=Cuenta");

            json = await _crmService.Get(uri.ToString());

            var contrato = JsonSerializer.Deserialize<List<ContratosIdDto>>(json)[0];

            if (!empresasDisponibles.Contains(contrato.Cuenta.id))
                return [];

            uri = new StringBuilder("crm/v2/Purchase_Orders/search?criteria=(Vehiculo.name:equals:" + dominio + ")" +
                "&fields=id,Clasificaciones,Vehiculo,Product_Details,Vendor_Name,Turno,Status,PO_Number");

            json = await _crmService.Get(uri.ToString());
            var operaciones = JsonSerializer.Deserialize<List<OperacionesResponseDto>>(json);

            var response = operaciones.Select(o => new OperacionesVehiculoDto
            {
                Id = o.Id,
                TipoOperacion = o.TipoOperacion,
                Detalle = o.Detalle.Any() ? o.Detalle.Select(d => d.Product.Name).ToList() : null,
                Taller = o.Taller?.name,
                FechaTurno = o.FechaTurno,
                Estado = o.Estado,
                OT = o.OT
            }).ToList();

            return response;
        }

    }
}