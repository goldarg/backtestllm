using System.Security.Claims;
using api.DataAccess;
using api.Models.DTO.Rol;
using api.Models.Entities;

namespace api.Services
{
    public class RolService : IRolService
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IRdaUnitOfWork _unitOfWork;

        public RolService(IUserIdentityService userIdentityService, IRdaUnitOfWork unitOfWork)
        {
            _userIdentityService = userIdentityService;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<RolDto>? GetAll(ClaimsPrincipal User)
        {
            var roles = _userIdentityService.ListarRolesInferiores(User);
            return roles.Select(x => new RolDto { Id = x.id, NombreRol = x.nombreRol });
        }

        public Rol? GetById(int id)
            => _unitOfWork.GetRepository<Rol>().GetById(id);
    }
}