using api.DataAccess;
using api.Models.DTO.Rol;
using api.Models.Entities;

namespace api.Services;

public class RolService : IRolService
{
    private readonly IUserIdentityService _userIdentityService;
    private readonly IRdaUnitOfWork _unitOfWork;

    public RolService(IUserIdentityService userIdentityService, IRdaUnitOfWork unitOfWork)
    {
        _userIdentityService = userIdentityService;
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<RolDto>? GetInferiores()
    {
        var roles = _userIdentityService.ListarRolesInferiores();
        return roles.Select(x => new RolDto { Id = x.id, NombreRol = x.nombreRol });
    }

    public IEnumerable<RolDto>? GetPropios()
    {
        var roles = _userIdentityService.ListarRolesDelUsuario();

        var rolesDb = _unitOfWork
            .GetRepository<Rol>()
            .GetAll()
            .Where(x => roles.Contains(x.nombreRol))
            .ToList();
        return rolesDb.Select(x => new RolDto { Id = x.id, NombreRol = x.nombreRol });
    }

    public Rol? GetById(int id)
    {
        return _unitOfWork.GetRepository<Rol>().GetById(id);
    }
}
