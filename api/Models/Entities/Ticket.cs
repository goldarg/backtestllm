namespace api.Models.Entities
{
    public class Ticket
    {
        public int id { get; set; }
        public int empresaId { get; set; }
        public required string dominioCrmId { get; set; }
        public required string dominio { get; set; }
        public required string departamentoCrmId { get; set; }
        public required string tipoOperacion { get; set; }
        public required string asunto { get; set; }
        public required string zona { get; set; }
        public required string descripcion { get; set; }
        public int odometro { get; set; }
        public DateTime turnoOpcion1 { get; set; }
        public DateTime turnoOpcion2 { get; set; }
        public required string idTiquetera { get; set; }
        public required string numeroTicket { get; set; }
        public int solicitanteId { get; set; }

        public virtual Empresa? Empresa { get; set; }
        public virtual User? Solicitante { get; set; }
    }
}
