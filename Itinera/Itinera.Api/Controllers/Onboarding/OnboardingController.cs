using Itinera.Application.Onboarding.Dtos;
using Itinera.Application.Onboarding.Interfaces;
using Itinera.Security.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Itinera.Api.Controllers.Onboarding;

[ApiController]
[Route("api/onboarding")]
[Authorize(Policy = Permisos.PlataformaGestionarEmpresas)]
public class OnboardingController(IOnboardingService onboardingService) : ControllerBase
{
    [HttpPost("empresas")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarEmpresa(RegistrarEmpresaRequest request)
    {
        var empresaId = await onboardingService.RegistrarEmpresaAsync(request);

        return Created(
            $"api/onboarding/empresas/{empresaId}",
            new { EmpresaId = empresaId });
    }
}

