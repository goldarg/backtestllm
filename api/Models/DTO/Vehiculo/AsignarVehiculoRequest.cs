namespace api.Models.DTO.Vehiculo
{
    public class AsignarVehiculoRequest
    {
        public int contratoId { get; set; }
        public int usuarioId { get; set; }
        public string tipoContrato { get; set; }
    }
}