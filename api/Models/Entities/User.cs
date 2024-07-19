namespace api.Models.Entities;

public class User
{
    public int id { get; set; }
    public string? userName { get; set; }
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public bool activo { get; set; }
    public bool isRDA { get; set; }
    public string? idCRM { get; set; }
    public Guid guid { get; set; }
    public virtual ICollection<UsuariosRoles>? Roles { get; set; }
    public virtual ICollection<UsuariosEmpresas>? EmpresasAsignaciones { get; set; }
}