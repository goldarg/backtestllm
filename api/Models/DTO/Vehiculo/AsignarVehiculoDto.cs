using api.Exceptions;

namespace api.Models.DTO.Vehiculo;

public class AsignarVehiculoDto
{
    public string? usuarioId { get; set; }
    public string tipoContrato { get; set; }
    public string? idContratoInterno { get; set; }
    public string? vehiculoId { get; set; }
    public string? dominio { get; set; }

    public string GetTipoContratoCrm()
    {
        if (tipoContrato == "Fleet Management")
            return "Servicios_RDA";
        else if (tipoContrato == "Renting")
            return "Renting";
        else if (tipoContrato == "Alquiler Corporativo")
            return "Alquileres";
        else
            throw new BadRequestException("No se pudo determinar el tipo de contrato del vehículo");
    }
}
