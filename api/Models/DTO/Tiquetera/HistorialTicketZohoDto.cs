using Newtonsoft.Json;

namespace api.Models.DTO.Tiquetera;

public class HistorialTicketDto
{
    public required string Id { get; set; }
    public string Resumen { get; set; }
    public string? Contenido { get; set; }
    public string Remitente { get; set; }
    public string FechaCreacion { get; set; }
}

public class HistorialTicketZohoDto
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("summary")]
    public string Resumen { get; set; }

    [JsonProperty("content")]
    public string? Contenido { get; set; }

    [JsonProperty("author")]
    public AuthorZohoDto Autor { get; set; }

    [JsonProperty("createdTime")]
    public string FechaCreacion { get; set; }

    // utilizado para filtrar

    [JsonProperty("visibility")]
    public string Visibility { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    // definiciones clases axuliares
    public class AuthorZohoDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
