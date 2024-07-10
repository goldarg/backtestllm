using System;
using System.Collections.Generic;

namespace YourNamespace
{

    public class Approval
    {
        public bool Delegate { get; set; }
        public bool Approve { get; set; }
        public bool Reject { get; set; }
        public bool Resubmit { get; set; }
    }

    public class ReviewProcess
    {
        public bool Approve { get; set; }
        public bool Reject { get; set; }
        public bool Resubmit { get; set; }
    }

    public class Persona
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
    }

    public class VehiculoDto
    {
        public Persona Owner { get; set; }
        public string EstadoRegistroTelemetria { get; set; }
        public string VehiculoRetirado { get; set; }
        public string PropietarioDeBaja { get; set; }
        public string State { get; set; }
        public string EnergA { get; set; }
        public bool ProcessFlow { get; set; }
        public string Agencia { get; set; }
        public string Currency { get; set; }
        public DateTime? FechaDeCompra { get; set; }
        public string Id { get; set; }
        public Approval Approval { get; set; }
        public DateTime? FechaDeActivacion { get; set; }
        public string Motor { get; set; }
        public DateTime CreatedTime { get; set; }
        public string IncluyeTelemetria { get; set; }
        public DateTime? FechaDeEntregaAlCliente { get; set; }
        public string ModificacionAprobadaPor { get; set; }
        public string Titularidad { get; set; }
        public DateTime? FechaDeInactivacion { get; set; }
        public string Modelo { get; set; }
        public string TipoDeCompra { get; set; }
        public DateTime? VtoCedulaVerde { get; set; }
        public string FranquiciaCooperacion { get; set; }
        public string Aprobador { get; set; }
        public Persona CreatedBy { get; set; }
        public bool GNC { get; set; }
        public DateTime? FechaDeSiniestro { get; set; }
        public string MarcaVehiculo { get; set; }
        public string Telemetrias { get; set; }
        public decimal? ValorDeCompra { get; set; }
        public bool Leasing { get; set; }
        public ReviewProcess ReviewProcess { get; set; }
        public string EnvioMiFlota { get; set; }
        public bool TieneCedulaAutorizado { get; set; }
        public DateTime? VencimientoDeRuta { get; set; }
        public DateTime? VencimientoDeCoberturaSOAT { get; set; }
        public string CompaniaDeSeguroSOA { get; set; }
        public string EstadoAccesorios { get; set; }
        public string Franquicia { get; set; }
        public DateTime? VencimientoMatafuego { get; set; }
        public bool Orchestration { get; set; }
        public DateTime? FechaSiguienteVTV { get; set; }
        public string PolizaN { get; set; }
        public string Tipo { get; set; }
        public DateTime? FechaEstimadaRecepcionDeDocumentacion { get; set; }
        public string Padron { get; set; }
        public DateTime? VtoCobertura { get; set; }
        public bool LockedS { get; set; }
        public string Cedulas { get; set; }
        public List<string> Tag { get; set; }
        public decimal? Pagado { get; set; }
        public bool TelemetriaInstalado { get; set; }
        public string CompaniaDeSeguroSOAT { get; set; }
        public DateTime? FechaDePago { get; set; }
        public string CurrencySymbol { get; set; }
        public DateTime? VencimientoMesVTV { get; set; }
        public string ProveedorVehiculo { get; set; }
        public string NumeroDePolizaSOAT { get; set; }
        public DateTime? VencimientoDeCoberturaSOA { get; set; }
        public string EnvioAFinnegans { get; set; }
        public string PolizaSeguro { get; set; }
        public string Name { get; set; }
        public DateTime LastActivityTime { get; set; }
        public string FCDeCompraN { get; set; }
        public decimal ExchangeRate { get; set; }
        public bool LockedForMe { get; set; }
        public bool Approved { get; set; }
        public bool Blindaje { get; set; }
        public decimal? ValorDelVehiculo { get; set; }
        public string Color { get; set; }
        public bool Editable { get; set; }
        public string MedidaCubierta { get; set; }
        public DateTime? FechaDeDisponibilidadDeLaUnidad { get; set; }
        public string EntregaDocumentacion { get; set; }
        public bool VTVITV { get; set; }
        public string ZiaOwnerAssignment { get; set; }
        public DateTime? FechaDeInstalacion { get; set; }
        public string NumeroDePolizaSOA { get; set; }
        public decimal? DiasEsperandoFondos { get; set; }
        public string Estado { get; set; }
        public string TipoCobertura { get; set; }
        public string CompaniaDeSeguro { get; set; }
        public Persona ModifiedBy { get; set; }
    }
}
