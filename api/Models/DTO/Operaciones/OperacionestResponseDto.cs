using System.Text.Json.Serialization;

namespace api.Models.DTO.Operaciones
{
    public class OperacionesResponseDto
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("Clasificaciones")]
        public string? TipoOperacion { get; set; }
        public CRMRelatedObject? Vehiculo { get; set; }

        [JsonPropertyName("Product_Details")]
        public List<ProductDetail>? Detalle { get; set; }

        [JsonPropertyName("Vendor_Name")]
        public CRMRelatedObject? Taller { get; set; }

        [JsonPropertyName("Turno")]
        public DateTime? FechaTurno { get; set; }

        [JsonPropertyName("Estado_OT_Mirai_fleet")]
        public string? Estado { get; set; }

        [JsonPropertyName("PO_Number")]
        public string? OT { get; set; }
    } // Agregamos las props a demanda

    public class ProductDetail
    {
        [JsonPropertyName("product")]
        public Product? Product { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("total_after_discount")]
        public decimal TotalAfterDiscount { get; set; }

        [JsonPropertyName("net_total")]
        public decimal NetTotal { get; set; }

        [JsonPropertyName("book")]
        public object Book { get; set; }

        [JsonPropertyName("tax")]
        public decimal Tax { get; set; }

        [JsonPropertyName("list_price")]
        public decimal ListPrice { get; set; }

        [JsonPropertyName("unit_price")]
        public object UnitPrice { get; set; }

        [JsonPropertyName("quantity_in_stock")]
        public int QuantityInStock { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("Product_Description")]
        public string ProductDescription { get; set; }

        [JsonPropertyName("Line_tax")]
        public List<object> LineTax { get; set; }
    }

    public class Product
    {
        [JsonPropertyName("product_code")]
        public string? Product_Code { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
