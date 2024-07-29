using api.Models.DTO.Rol;
using api.Models.Entities;

namespace api.Services
{
    public interface IRolService
    {
        IEnumerable<RolDto>? GetAll();
        Rol? GetById(int id);
    }
}