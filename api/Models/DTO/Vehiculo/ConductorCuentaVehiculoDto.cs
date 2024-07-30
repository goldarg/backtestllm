namespace api.Models.DTO.Vehiculo
{
    public class ConductorCuentaVehiculoDto
    {
        public CRMRelatedObject Conductor { get; set; }
        public CRMRelatedObject Dominio { get; set; }
        public CRMRelatedObject Contrato { get; set; }
        public string estadoContratoInterno { get; set; }
        public string contratoIdInterno { get; set; }
        public string? Plazo_Propuesta { get; set; }
        public DateTime? FechaFinContratoInterno { get; set; }

        // traido desde contratos internos
        public string? Centro_de_costos { get; set; }

        // traido desde contratos internos
        public string? Sector { get; set; }
    }
}
