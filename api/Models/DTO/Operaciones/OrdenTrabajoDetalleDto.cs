using Newtonsoft.Json;

namespace api.Models.DTO.Operaciones
{
    public class OrdenTrabajoDetalleDto
    {
        //Campos generales de la OT
        [JsonProperty("id")]
        public string? crmId { get; set; }

        [JsonProperty("PO_Number")]
        public string? nroOT { get; set; }

        [JsonProperty("Estado_OT_Mirai_fleet")]
        public string? estadoOT { get; set; }

        [JsonProperty("Motivo_de_rechazo_OT")]
        public string? motivoRechazoOT { get; set; }

        [JsonProperty("Turno")]
        public DateTime? turno { get; set; }

        [JsonProperty("Vendor_Name")]
        public CRMRelatedObject? Taller { get; set; }

        [JsonProperty("Domicilio_Completo")]
        public string? direccionTaller { get; set; }

        [JsonProperty("Departamento")]
        public string? departamento { get; set; }

        [JsonProperty("Clasificaciones")]
        public string? clasificaciones { get; set; }

        [JsonProperty("Tracking_Number")]
        public string? numeroTicket { get; set; }

        [JsonProperty("Created_Time")]
        public DateTime? fechaCreacionOT { get; set; }

        [JsonProperty("Fecha_de_Aprobaci_n")]
        public DateTime? fechaAprobacion { get; set; }

        [JsonProperty("Estado_de_presupuesto")]
        public string? estadoPresupuesto { get; set; }

        [JsonProperty("Od_metro")]
        public double? lecturaOdometro { get; set; }

        [JsonProperty("Grand_Total")]
        public double? totalPresupuesto { get; set; }

        [JsonProperty("Aprobador")]
        public CRMRelatedObject? Aprobador { get; set; }

        [JsonProperty("Solicitante")]
        public CRMRelatedObject? Solicitante { get; set; }

        [JsonProperty("Cliente")]
        public CRMRelatedObject? Cliente { get; set; }

        [JsonProperty("Vehiculo")]
        public CRMRelatedObject? Vehiculo { get; set; }

        [JsonProperty("Product_Details")]
        public List<DetalleOTDetalleIntervencionDto> DetalleIntervencion { get; set; }

        public DatosConductorDto Conductor { get; set; }

        public DatosDominioDto Dominio { get; set; }
    }

    public class DatosConductorDto
    {
        [JsonProperty("id")]
        public string? crmId { get; set; }

        [JsonProperty("Full_Name")]
        public string? fullName { get; set; }

        [JsonProperty("Email")]
        public string? mail { get; set; }

        [JsonProperty("Phone")]
        public string? telefono { get; set; }
    }

    public class DatosDominioDto
    {
        [JsonProperty("id")]
        public string? crmId { get; set; }

        [JsonProperty("Versi_n")]
        public string? version { get; set; }

        [JsonProperty("Marca_Vehiculo")]
        public string? marca { get; set; }

        [JsonProperty("Modelo")]
        public string? modelo { get; set; }

        [JsonProperty("Pa_s")]
        public string? pais { get; set; }

        [JsonProperty("Name")]
        public string? dominio { get; set; }
    }

    public class DetalleOTDetalleIntervencionDto
    {
        [JsonProperty("product")]
        public DetalleProductDto producto { get; set; }

        [JsonProperty("unit_price")]
        public double? precioPorUnidad { get; set; }

        [JsonProperty("quantity")]
        public int? cantidad { get; set; }

        [JsonProperty("net_total")]
        public double? total { get; set; }
    }

    public class DetalleProductDto
    {
        [JsonProperty("name")]
        public string? catalogoTrabajo { get; set; }

        [JsonProperty("id")]
        public string? id { get; set; }
    }
}
