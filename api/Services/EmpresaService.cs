using api.DataAccess;
using api.Models.DTO.Empresa;
using api.Models.Entities;

namespace api.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IRdaUnitOfWork _unitOfWork;

        public EmpresaService(IUserIdentityService userIdentityService, IRdaUnitOfWork unitOfWork)
        {
            _userIdentityService = userIdentityService;
            _unitOfWork = unitOfWork;
        }

        public List<EmpresaDto> GetAll()
        {
            var empresasUsuario = _userIdentityService.ListarEmpresasDelUsuario();

            return _unitOfWork
                .GetRepository<Empresa>()
                .GetAll()
                .Where(x => empresasUsuario.Contains(x.idCRM))
                .Select(x => new EmpresaDto { IdCRM = x.idCRM, RazonSocial = x.razonSocial })
                .ToList();
        }

        public Empresa? GetById(int id)
            => _unitOfWork.GetRepository<Empresa>().GetById(id);
    }
}