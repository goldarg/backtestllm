using System;
using System.Collections.Generic;

namespace api.Models.DTO
{

    public class VehiculoResponse
    {
        public List<VehiculoDto> Data { get; set; }
    }

    public class Conductor
    {
        public string id { get; set;}
        public string name { get; set; }
    }

    public class VehiculoDto
    {
        public string id { get; set; }
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
        public Conductor? Conductor { get; set; }

//Cuenta
//Grupo
//Poliza
//Cobertura
//Fecha_siguiente_VTV
//Conductor
//Empresa
    }
}
