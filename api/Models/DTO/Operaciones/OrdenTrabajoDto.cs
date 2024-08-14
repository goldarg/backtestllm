using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace api.Models.DTO.Operaciones
{
    public class OrdenTrabajoDto
    {
        [JsonProperty("Tracking_Number")]
        public string? numeroTicket { get; set; }

        [JsonProperty("PO_Number")]
        public string? numeroOT { get; set; }

        [JsonProperty("Estado_OT_Mirai_fleet")]
        public string? estadoOT { get; set; }

        [JsonProperty("Clasificaci_n")]
        public string? tipoOperacion { get; set; }

        [JsonProperty("Vehiculo")]
        public CRMRelatedObject? Vehiculo { get; set; }

        [JsonProperty("Cliente")]
        public CRMRelatedObject? Cliente { get; set; }

        [JsonProperty("Product_Details")]
        public List<DetalleIntervencionDto> DetallesIntervencion { get; set; }

        [JsonProperty("Aprobador")]
        public CRMRelatedObject? Aprobador { get; set; }

        [JsonProperty("Vendor_Name")]
        public CRMRelatedObject? Taller { get; set; }

        [JsonProperty("Solicitante")]
        public CRMRelatedObject? conductor { get; set; }

        [JsonProperty("Estado_de_presupuesto")]
        public string? presupuesto { get; set; }
    }

    public class DetalleIntervencionDto
    {
        public ProductDto product { get; set; }
    }

    public class ProductDto
    {
        public string? Product_Code { get; set; }
        public string? Currency { get; set; }
        public string? name { get; set; }
        public string? id { get; set; }
    }
}
