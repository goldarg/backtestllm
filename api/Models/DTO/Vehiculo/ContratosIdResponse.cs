namespace api.Models.DTO.Vehiculo
{
    public class ContratosIdDto
    {
        public string id { get; set; }
        public string Tipo_de_Contrato { get; set; }
        public CRMRelatedObject Cuenta { get; set; }
    }
}