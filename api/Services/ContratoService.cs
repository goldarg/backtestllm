using System.Text;
using api.Connected_Services;
using api.Models.DTO.Contrato;
using Newtonsoft.Json;

namespace api.Services;

public class ContratoService : IContratoService
{
    private readonly IUserIdentityService _identityService;
    private readonly CRMService _crmService;

    public ContratoService(IUserIdentityService identityService, CRMService crmService)
    {
        _identityService = identityService;
        _crmService = crmService;
    }

    public async Task<ContratosResponse?> GetContratos()
    {
        var empresasDisponibles = _identityService.ListarEmpresasDelUsuario();

        var accountCriteria = string.Empty;
        if (empresasDisponibles.Length <= 10)
        {
            accountCriteria = string.Join(
                "or",
                empresasDisponibles.Select(idCrm => $"(Cuenta.id:equals:{idCrm})")
            );
            accountCriteria = $"and({accountCriteria})";
        }

        var tipoContratoCriteria =
            "((Tipo_de_Contrato:equals:Renting)or(Tipo_de_Contrato:equals:Fleet Management)or(Tipo_de_Contrato:equals:Alquiler Corporativo)or(Tipo_de_Contrato:equals:Telemetria))";
        var criteria = $"{tipoContratoCriteria}{accountCriteria}";
        var fields =
            "id,Cuenta,Tipo_de_Contrato,Plazo_Propuesta,Servicios,Gesti_n,Infracciones_Servicio,Seguro,Telemetr_a_Servicio";
        var uri = new StringBuilder($"crm/v2/Contratos/search?criteria={criteria}&fields={fields}");

        var responseString = await _crmService.Get(uri.ToString());

        // Deserializar el JSON directamente a List<ContratoResponse>
        var contratosMarcosDto = JsonConvert.DeserializeObject<List<ContratoMarcoDto>>(
            responseString
        );

        if (contratosMarcosDto == null)
            return null;

        // Filtrar los contratos desde el BE si empresasDisponibles tiene más de 10 IDs
        if (empresasDisponibles.Length > 10)
            contratosMarcosDto = contratosMarcosDto
                .Where(x => empresasDisponibles.Contains(x.Cuenta?.id))
                .ToList();

        var contratosMarcoDict = contratosMarcosDto
            .Where(c => !string.IsNullOrEmpty(c.id))
            .ToDictionary(c => c.id ?? "");

        var tRenting = GetModulosIntermedios<ContratoRentingDto>(
            "crm/v2/Renting?fields=id,Fecha_inicio_renting,Fecha_fin_de_renting,Canon,Dominio,Fecha_de_extensi_n_del_Renting,Nombre_del_contrato",
            contratosMarcoDict
        );
        var tServicioRda = GetModulosIntermedios<ContratoServicioRdaDto>(
            "crm/v2/Servicios_RDA?fields=id,Inicio_de_servicio,Fin_de_servicio,Dominio,Fee_por_auto,Contrato",
            contratosMarcoDict
        );
        var tAlquiler = GetModulosIntermedios<ContratoAlquilerDto>(
            "crm/v2/Alquileres?fields=id,Contrato,Fecha_de_Entrega,Fecha_de_Devolucion,Dominio_Alquiler",
            contratosMarcoDict
        );
        var tTelemetria = GetModulosIntermedios<ContratoTelemetriaDto>(
            "crm/v2/Telemetrias/search?criteria=(Tipo_de_Contrato:equals:Telemetria)&fields=id,Nombre_del_contrato,Fecha,Fecha_de_Fin,Dominio_vehiculo,Fee_por_auto",
            contratosMarcoDict
        );

        await Task.WhenAll(tRenting, tServicioRda, tAlquiler, tTelemetria);

        var contratosRenting = await tRenting;
        var contratosServicioRda = await tServicioRda;
        var contratosAlquiler = await tAlquiler;
        var contratosTelemetria = await tTelemetria;
        // modificar
        return new ContratosResponse(
            contratosRenting,
            contratosServicioRda,
            contratosAlquiler,
            contratosTelemetria
        );
    }

    private async Task<List<T>> GetModulosIntermedios<T>(
        string endpoint,
        Dictionary<string, ContratoMarcoDto> contratosMarcoDict
    )
        where T : ContratoBaseDto
    {
        var json = await _crmService.Get(endpoint);
        var dtos = JsonConvert.DeserializeObject<List<T>>(json);
        if (dtos == null)
            return new List<T>();

        var dtosFiltrados = FiltrarEInyectarContratoPadre(dtos, contratosMarcoDict);
        return dtosFiltrados;
    }

    private List<T> FiltrarEInyectarContratoPadre<T>(
        List<T> dtos,
        Dictionary<string, ContratoMarcoDto> contratosMarcoDict
    )
        where T : ContratoBaseDto
    {
        var dtosFiltrados = new List<T>();

        foreach (var dto in dtos)
            if (
                !string.IsNullOrEmpty(dto.ContratoPadre?.id)
                && contratosMarcoDict.TryGetValue(dto.ContratoPadre.id, out var contratoPadre)
            )
            {
                dto.ContratoMarco = contratoPadre;
                dtosFiltrados.Add(dto);
            }

        return dtosFiltrados;
    }
}
