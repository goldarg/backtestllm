using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using api.Models.DTO;

namespace api.Logic
{
    public class VehiculosLogica
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VehiculosLogica(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
    }
}