using Newtonsoft.Json;

namespace api.Models.DTO.Contrato;

public class ContratosResponse(
    List<ContratoRentingDto> contratosRenting,
    List<ContratoServicioRdaDto> contratosServicioRda,
    List<ContratoAlquilerDto> contratosAlquiler,
    List<ContratoTelemetriaDto> contratosTelemetria
)
{
    public List<ContratoRentingDto> ContratosRenting { get; set; } = contratosRenting;
    public List<ContratoServicioRdaDto> ContratosServicioRda { get; set; } = contratosServicioRda;
    public List<ContratoAlquilerDto> ContratosAlquiler { get; set; } = contratosAlquiler;
    public List<ContratoTelemetriaDto> ContratosTelemetria { get; set; } = contratosTelemetria;
}

/// <summary>
///     Utilizado para deserializar el JSON de la respuesta del CRM
/// </summary>
public class ContratoMarcoDto
{
    public string? id { get; set; }

    public string Estado { get; set; }

    [JsonProperty("Tipo_de_Contrato")]
    public string? TipoDeContrato { get; set; }

    [JsonProperty("Plazo_Propuesta")]
    public string? PlazoPropuesta { get; set; }

    // utiizado para filtrar los contratos marcos que se pueden ver
    public CRMRelatedObject? Cuenta { get; set; }

    // si es de tipo renting esta relleno
    [JsonProperty("Servicios")]
    public List<string>? Servicios { get; set; }

    // si es de tipo servicio_rda esta relleno
    [JsonProperty("Gesti_n")]
    public bool? ServicioIncluidoGestion { get; set; }

    [JsonProperty("Infracciones_Servicio")]
    public bool? ServicioIncluidoInfracciones { get; set; }

    [JsonProperty("Seguro")]
    public bool? ServicioIncluidoSeguro { get; set; }

    [JsonProperty("Telemetr_a_Servicio")]
    public bool? ServicioIncluidoTelemetria { get; set; }
}

public abstract class ContratoBaseDto
{
    public string id { get; set; }
    public virtual DateTime? FechaInicioContrato { get; set; }
    public virtual DateTime? FechaFinContrato { get; set; }

    public virtual CRMRelatedObject? Dominio { get; set; }

    // necesario para saber a que contrato pertenece, recibido del CRM
    public virtual CRMRelatedObject? ContratoPadre { get; set; }

    // inyectado en el codigo
    public ContratoMarcoDto ContratoMarco { get; set; }
}

public class ContratoRentingDto : ContratoBaseDto
{
    [JsonProperty("Fecha_inicio_renting")]
    public override DateTime? FechaInicioContrato { get; set; }

    [JsonProperty("Fecha_fin_de_renting")]
    public override DateTime? FechaFinContrato { get; set; }

    [JsonProperty("Canon")]
    public decimal? CanonMensual { get; set; }

    [JsonProperty("Fecha_de_extensi_n_del_Renting")]
    public DateTime? FechaVencimientoAdenda { get; set; }

    [JsonProperty("Nombre_del_contrato")]
    public override CRMRelatedObject? ContratoPadre { get; set; }
}

/// <summary>
///     Esto tambien se llama Fleet Management
/// </summary>
public class ContratoServicioRdaDto : ContratoBaseDto
{
    [JsonProperty("Inicio_de_servicio")]
    public override DateTime? FechaInicioContrato { get; set; }

    [JsonProperty("Fin_de_servicio")]
    public override DateTime? FechaFinContrato { get; set; }

    [JsonProperty("Fee_por_auto")]
    public decimal? FeeMensual { get; set; }

    // necesario para saber a que contrato pertenece
    [JsonProperty("Contrato")]
    public override CRMRelatedObject? ContratoPadre { get; set; }
}

public class ContratoAlquilerDto : ContratoBaseDto
{
    [JsonProperty("Fecha_de_Entrega")]
    public override DateTime? FechaInicioContrato { get; set; }

    [JsonProperty("Fecha_de_Devolucion")]
    public override DateTime? FechaFinContrato { get; set; }

    [JsonProperty("Dominio_Alquiler")]
    public override CRMRelatedObject? Dominio { get; set; }

    // necesario para saber a que contrato pertenece
    [JsonProperty("Contrato")]
    public override CRMRelatedObject? ContratoPadre { get; set; }
}

public class ContratoTelemetriaDto : ContratoBaseDto
{
    [JsonProperty("Fecha")]
    public override DateTime? FechaInicioContrato { get; set; }

    [JsonProperty("Fecha_de_Fin")]
    public override DateTime? FechaFinContrato { get; set; }

    [JsonProperty("Dominio_vehiculo")]
    public override CRMRelatedObject? Dominio { get; set; }

    // TODO Este campo tiene un poco mas de logica extraña
    [JsonProperty("Fee_por_auto")]
    public decimal? FeeMensual { get; set; }

    // necesario para saber a que contrato pertenece
    [JsonProperty("Nombre_del_contrato")]
    public override CRMRelatedObject? ContratoPadre { get; set; }
}
