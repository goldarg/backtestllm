using System;
using Newtonsoft.Json;

namespace api.Models.DTO;

public class VehiculoDto
{
    #region propiedades traidas del CRM tabla Vehiculos
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Estado")]
    public string? Estado { get; set; }

    [JsonProperty("Marca_Vehiculo")]
    public string? Marca_Vehiculo { get; set; }

    [JsonProperty("Modelo")]
    public string? Modelo { get; set; }

    [JsonProperty("Versi_n")]
    public string? Versi_n { get; set; }

    [JsonProperty("Chasis")]
    public string? Chasis { get; set; }

    [JsonProperty("Color")]
    public string? Color { get; set; }

    [JsonProperty("A_o")]
    public string? A_o { get; set; }

    [JsonProperty("Medida_Cubierta")]
    public string? Medida_Cubierta { get; set; }

    [JsonProperty("Fecha_de_patentamiento")]
    public DateTime? Fecha_de_patentamiento { get; set; }

    [JsonProperty("Compa_a_de_seguro")]
    public string? Compa_a_de_seguro { get; set; }

    [JsonProperty("Franquicia")]
    public string? Franquicia { get; set; }

    [JsonProperty("Poliza_N")]
    public string? Poliza_N { get; set; }

    [JsonProperty("Vencimiento_Matafuego")]
    public DateTime? Vencimiento_Matafuego { get; set; }

    [JsonProperty("Vencimiento_de_Ruta")]
    public DateTime? Vencimiento_de_Ruta { get; set; }

    [JsonProperty("Padron")]
    public string? Padron { get; set; }

    [JsonProperty("Vto_Cedula_verde")]
    public DateTime? Vto_Cedula_verde { get; set; }

    [JsonProperty("Ultimo_Odometro_KM")]
    public int? Ultimo_Odometro_KM { get; set; }

    [JsonProperty("Fecha_siguiente_VTV")]
    public DateTime? Fecha_siguiente_VTV { get; set; }

    [JsonProperty("Pa_s")]
    public string? Pa_s { get; set; }

    [JsonProperty("Tipo_cobertura")]
    public string? Tipo_cobertura { get; set; }
    #endregion

    #region propiedades inyectadas desde otras tablas del CRM
    public CRMRelatedObject? Conductor { get; set; }
    public CRMRelatedObject? Contrato { get; set; }
    public string? estadoContratoInterno { get; set; }
    public string? idContratoInterno { get; set; }
    public string? tipoContrato { get; set; }
    public CRMRelatedObject? Cuenta { get; set; }

    //Grupo = Holding, "padre" de la empresa que representa un grupo de empresas.
    //TODO de RDA: Falta que lo agreguen en el CRM, y venir a mappearlo aca (por ahora hardcodeado)
    public CRMRelatedObject? Grupo { get; set; }
    public string? plazoContrato { get; set; }
    public DateTime? fechaFinContratoInterno { get; set; }

    // traido desde contratos internos
    public string? Centro_de_costos { get; set; }

    // traido desde contratos internos
    public string? Sector { get; set; }
    #endregion
}
