using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Entities;

public class Rol
{
    public int id { get; set; }
    public string? nombreRol { get; set; }
    public Guid guid { get; set; }
    public int jerarquia { get; set; }
    public virtual ICollection<UsuariosRoles>? Asignaciones { get; set; }
}
