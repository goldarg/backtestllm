namespace api.Models.DTO.Vehiculo
{
    public class ConductorCuentaVehiculoDto
    {
        public CRMRelatedObject Conductor { get; set; }
        public CRMRelatedObject Dominio { get; set; }
        public CRMRelatedObject Contrato { get; set; }
    }
}