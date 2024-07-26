using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Entities;

public class UsuariosEmpresas
{
    public int id { get; set; }
    public int userId { get; set; }
    public int empresaId { get; set; }
    public virtual User? User { get; set; }
    public virtual Empresa? Empresa { get; set; }
}
