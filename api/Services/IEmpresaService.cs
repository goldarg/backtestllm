using api.Models.DTO.Empresa;
using api.Models.Entities;

namespace api.Services
{
    public interface IEmpresaService
    {
        List<EmpresaDto> GetAll();
        Empresa? GetById(int id);
    }
}