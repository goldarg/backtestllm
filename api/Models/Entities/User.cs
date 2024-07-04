namespace api.Models.Entities;

public class User
{
    public int id { get; set; }
    public string? userName { get; set; }
    public string? nombre { get; set; }
    public string? apellido { get; set; }
    public bool activo { get; set; }
    public int empresaId { get; set; }
    public Empresa? Empresa { get; set; }
    public Guid guid { get; set; }
    public ICollection<UsuariosRoles>? Roles { get; set; }
}