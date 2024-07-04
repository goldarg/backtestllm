using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Entities;

public class UsuariosRoles
{
    public int id { get; set; }
    public int userId { get; set; }
    public int rolId { get; set; }
    public User? User { get; set; }
    public Rol? Rol { get; set; }
}