using System.Security.Claims;
using System.Text;
using System.Text.Json;
using api.Connected_Services;
using api.DataAccess;
using api.Exceptions;
using api.Models.DTO.Contrato;
using api.Models.Entities;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
public class ContratosController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRdaUnitOfWork _unitOfWork;
    private readonly CRMService _crmService;
    private readonly IUserIdentityService _identityService;

    public ContratosController(
        IUserIdentityService identityService,
        IHttpClientFactory httpClientFactory,
        IRdaUnitOfWork unitOfWork,
        CRMService crmService
    )
    {
        _httpClientFactory = httpClientFactory;
        _unitOfWork = unitOfWork;
        _crmService = crmService;
        _identityService = identityService;
    }

    [HttpGet]
    [Authorize(Roles = "RDA,SUPERADMIN,ADMIN")]
    public async Task<IActionResult> GetContratos()
    {
        var empresasDisponibles = _identityService.ListarEmpresasDelUsuario(User);

        var uri = new StringBuilder("crm/v2/Contratos?fields=id,Name,Cuenta");
        var json = await _crmService.Get(uri.ToString());
        var contratos = JsonSerializer.Deserialize<List<ContratoResponse>>(json);

        //Se filtra desde el BE porque el CRM soporta un maximo de 20 operadores logicos
        var result = contratos.Where(x => empresasDisponibles.Contains(x.Cuenta.id)).ToList();

        return Ok(result);
    }
}
