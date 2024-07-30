using api.Models.DTO.Rol;
using api.Models.Entities;

namespace api.Services
{
    public interface IRolService
    {
        IEnumerable<RolDto>? GetInferiores();
        Rol? GetById(int id);
        IEnumerable<RolDto>? GetPropios();
    }
}
