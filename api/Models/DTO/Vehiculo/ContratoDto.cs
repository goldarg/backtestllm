using Newtonsoft.Json;

namespace api.Models.DTO.Vehiculo;

/// <summary>
/// Utilizado para deserealizacion del CRM, solo los campos requeridos para vehiculos
/// </summary>
public class ContratoDto
{
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("Tipo_de_Contrato")]
    public string Tipo_de_Contrato { get; set; }

    [JsonProperty("Cuenta")]
    public CRMRelatedObject Cuenta { get; set; }

    [JsonProperty("Estado")]
    public string Estado { get; set; }

    [JsonProperty("Plazo_Propuesta")]
    public string? Plazo_Propuesta { get; set; }
}
