using System.Security.Claims;
using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO.Contrato;
using api.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
public class ContratosController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly CRMService _crmService;

    public ContratosController(IHttpClientFactory httpClientFactory, IRdaUnitOfWork unitOfWork, CRMService crmService)
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _crmService = crmService;
    }

    [HttpGet]
    [Authorize(Roles = "CONDUCTOR")]
    public async Task<IActionResult> GetContratos()
    {
        var userId = User.Identity.Name; //TODO ver de donde sale el username o el ID

        // get roles from User token
        // TODO bonito, ponerlo en un Servicio, obtener cosas del contexto IHttpContextService
        var roles = User.Claims.Where(x => x.Type == (User.Identity as ClaimsIdentity).RoleClaimType)
            .Select(x => x.Value).ToList();

        return Ok("chau");

        var placeholder = 3;

        var requestUser = _unitOfWork.GetRepository<User>().GetAll()
            .Where(x => x.id == placeholder)
            .SingleOrDefault();

        if (requestUser == null)
            throw new BadRequestException("No se encontró el usuario solicitante");

        var empresasDisponibles = requestUser.EmpresasAsignaciones.Select(x => x.Empresa.idCRM).ToList();

        var uri = new StringBuilder("crm/v2/Contratos?fields=id,Name,Cuenta");
        var json = await _crmService.Get(uri.ToString());
        var contratos = JsonSerializer.Deserialize<List<ContratoResponse>>(json);

        //Se filtra desde el BE porque el CRM soporta un maximo de 20 operadores logicos
        var result = contratos.Where(x => empresasDisponibles.Contains(x.Cuenta.id)).ToList();

        return Ok(result);
    }
}