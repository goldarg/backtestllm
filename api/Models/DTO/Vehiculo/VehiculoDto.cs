namespace api.Models.DTO
{
    public class VehiculoDto
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string? Estado { get; set; }
        public string? Marca_Vehiculo { get; set; }
        public string? Modelo { get; set; }
        public string? Versi_n { get; set; }
        public string? Chasis { get; set; }
        public string? Color { get; set; }
        public string? A_o { get; set; }
        public string? Medida_Cubierta { get; set; }
        public DateTime? Fecha_de_patentamiento { get; set; }
        public string? Compa_a_de_seguro { get; set; }
        public string? Franquicia { get; set; }
        public string? Poliza_N { get; set; }
        public DateTime? Vencimiento_Matafuego { get; set; }
        public DateTime? Vencimiento_de_Ruta { get; set; }
        public string? Padron { get; set; }
        public DateTime? Vto_Cedula_verde { get; set; }
        public CRMRelatedObject? Conductor { get; set; }
        public CRMRelatedObject? Contrato { get; set; }
        public string? estadoContratoInterno { get; set; }
        public string? idContratoInterno { get; set; }
        public string? tipoContrato { get; set; }
        public CRMRelatedObject? Cuenta { get; set; }
        public int? Ultimo_Odometro_KM { get; set; }

        //Grupo = Holding, "padre" de la empresa que representa un grupo de empresas.
        //TODO de RDA: Falta que lo agreguen en el CRM, y venir a mappearlo aca (por ahora hardcodeado)
        public CRMRelatedObject? Grupo { get; set; }
        public DateTime? Fecha_siguiente_VTV { get; set; }
        public string? Pa_s { get; set; }
        public string? Tipo_cobertura { get; set; }
        public string? plazoContrato { get; set; }
        public DateTime? fechaFinContratoInterno { get; set; }

        // traido desde contratos internos
        public string? Centro_de_costos { get; set; }

        // traido desde contratos internos
        public string? Sector { get; set; }

        //Poliza
        //Cobertura
    }
}
