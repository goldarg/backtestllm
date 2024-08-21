namespace api.Models.DTO.Tiquetera;

public class TicketDtoResponse
{
    public string? numeroTicket { get; set; }
    public string? tipoOperacion { get; set; }

    public CRMRelatedObject? dominio { get; set; }
    public UserDtoResponse? solicitante { get; set; }
}

public class UserDtoResponse
{
    // id crm
    public string? id { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
}
