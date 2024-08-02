using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Models.DTO.ActividadUsuarios;
using api.Models.DTO.Contrato;
using api.Models.Entities;

namespace api.Services
{
    public class ActividadUsuarioService : IActividadUsuarioService
    {
        private readonly IUserIdentityService _identityService;
        private readonly CRMService _crmService;
        private readonly IRdaUnitOfWork _unitOfWork;

        public ActividadUsuarioService(
            IRdaUnitOfWork unitOfWork,
            IUserIdentityService identityService,
            CRMService crmService
        )
        {
            _identityService = identityService;
            _crmService = crmService;
            _unitOfWork = unitOfWork;
        }

        public void CrearRegistroActividad(
            int usuarioAfectadoId,
            int usuarioEjecutorId,
            string descripcion
        )
        {
            var userDb = _identityService.GetUsuarioDb();
            var newActividad = new ActividadUsuario
            {
                descripcion = descripcion,
                usuarioAfectadoId = usuarioAfectadoId,
                fecha = DateTime.Now,
                usuarioEjecutorId = usuarioEjecutorId,
            };

            _unitOfWork.GetRepository<ActividadUsuario>().Insert(newActividad);
            _unitOfWork.SaveChanges();
        }

        public void CrearActividadDb(int usuarioAfectadoId, string descripcion)
        {
            var userDb = _identityService.GetUsuarioDb();
            CrearRegistroActividad(usuarioAfectadoId, userDb.id, descripcion);
        }

        public void CrearActividadCrm(string usuarioAfectadoCrmId, string descripcion)
        {
            var usuarioAfectadoId = _unitOfWork
                .GetRepository<User>()
                .GetAll()
                .Where(x => x.idCRM == usuarioAfectadoCrmId)
                .Select(x => x.id)
                .FirstOrDefault();

            var userDb = _identityService.GetUsuarioDb();
            CrearRegistroActividad(usuarioAfectadoId, userDb.id, descripcion);
        }

        public IList<ActividadUsuarioDto> GetActividadUsuarioAfectado()
        {
            var userDb = _identityService.GetUsuarioDb();

            var result = _unitOfWork
                .GetRepository<ActividadUsuario>()
                .GetAll()
                .Where(x => x.usuarioAfectadoId == userDb.id)
                .Select(x => new ActividadUsuarioDto
                {
                    id = x.id,
                    descripcion = x.descripcion,
                    fecha = x.fecha,
                    usuarioAfectadoId = x.usuarioAfectadoId,
                    usuarioEjecutorId = x.usuarioEjecutorId,
                    usuarioAfectadoNombre = x.usuarioAfectado.userName,
                    usuarioEjecutorNombre = x.usuarioEjecutor.userName
                })
                .ToList();

            return result;
        }

        public IList<ActividadUsuarioDto> GetActividadUsuarioEjecutor()
        {
            var userDb = _identityService.GetUsuarioDb();

            var result = _unitOfWork
                .GetRepository<ActividadUsuario>()
                .GetAll()
                .Where(x => x.usuarioEjecutorId == userDb.id)
                .Select(x => new ActividadUsuarioDto
                {
                    id = x.id,
                    descripcion = x.descripcion,
                    fecha = x.fecha,
                    usuarioAfectadoId = x.usuarioAfectadoId,
                    usuarioEjecutorId = x.usuarioEjecutorId,
                    usuarioAfectadoNombre = x.usuarioAfectado.userName,
                    usuarioEjecutorNombre = x.usuarioEjecutor.userName
                })
                .ToList();

            return result;
        }
    }
}
